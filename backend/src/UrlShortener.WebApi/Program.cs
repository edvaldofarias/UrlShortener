using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Infra.Context;
using UrlShortener.WebApi.Infra.Repositories;
using UrlShortener.WebApi.Infra.Repositories.Interfaces;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddScoped<IShortenRepository, ShortenRepository>();
builder.Services.AddScoped<ISequenceRepository, SequenceRepository>();
builder.Services.AddMemoryCache(options =>
{    
    options.ExpirationScanFrequency = TimeSpan.FromSeconds(30);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var configPostgre = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
{
    Pooling = true,
    MinPoolSize = 2,
    MaxPoolSize = 8,
    ConnectionIdleLifetime = 100, // segundos
    Timeout = 15 // tempo de abertura de conexão
};

builder.Services.AddDbContextPool<UrlShortenerContext>(options =>
{
    options.UseNpgsql(configPostgre.ConnectionString,  npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
}, poolSize: 32);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<UrlShortenerContext>();
        // aguarda / tenta conectar com retry simples
        await WaitForDatabaseAsync(db, TimeSpan.FromSeconds(30));
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        // logue e opcionalmente rethrow para a app falhar ao subir
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Erro ao migrar o banco");
        throw;
    }
}

app.Run();

// Função de retry simples (opcional, veja abaixo)
static async Task WaitForDatabaseAsync(DbContext db, TimeSpan timeout)
{
    var sw = Stopwatch.StartNew();
    while (true)
    {
        try
        {
            await db.Database.CanConnectAsync();
            return;
        }
        catch
        {
            if (sw.Elapsed > timeout) throw;
            await Task.Delay(1000);
        }
    }
}
