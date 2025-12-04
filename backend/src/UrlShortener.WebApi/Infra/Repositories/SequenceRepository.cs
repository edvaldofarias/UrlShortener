using System.Data;
using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Infra.Context;
using UrlShortener.WebApi.Infra.Repositories.Interfaces;

namespace UrlShortener.WebApi.Infra.Repositories;

public class SequenceRepository(UrlShortenerContext context) : ISequenceRepository
{
    public async Task<long> GetNextSequenceValueAsync(string sequenceName, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT nextval('\"{sequenceName}\"')";

        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result);
    }
}