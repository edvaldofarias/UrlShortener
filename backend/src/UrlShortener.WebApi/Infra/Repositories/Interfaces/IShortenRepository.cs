using UrlShortener.WebApi.Entities.Shorten;

namespace UrlShortener.WebApi.Infra.Repositories.Interfaces
{
    /// <summary>
    /// Repositório para operações sobre entidades de URL encurtada.
    /// </summary>
    public interface IShortenRepository
    {
        /// <summary>
        /// Adiciona uma entrada de URL encurtada de forma assíncrona.
        /// </summary>
        Task AddAsync(Shorten shorten, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera uma entrada de URL encurtada pelo código curto.
        /// Retorna null se não for encontrada.
        /// </summary>
        Task<Shorten?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera uma entrada de URL encurtada pela URL longa.
        /// Retorna null se não for encontrada.
        /// </summary>
        Task<Shorten?> GetByLongUrlAsync(string longUrl, CancellationToken cancellationToken = default);
    }
}