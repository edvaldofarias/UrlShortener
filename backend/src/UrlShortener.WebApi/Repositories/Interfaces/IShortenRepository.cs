using UrlShortener.WebApi.Entities.Shorten;

namespace UrlShortener.WebApi.Repositories.Interfaces
{
    /// <summary>
    /// Repositório para operações sobre entidades de URL encurtada.
    /// </summary>
    public interface IShortenRepository
    {
        /// <summary>
        /// Adiciona uma entrada de URL encurtada de forma assíncrona.
        /// </summary>
        Task AddAsync(ShortenEntity shorten, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera uma entrada de URL encurtada pelo código curto.
        /// Retorna null se não for encontrada.
        /// </summary>
        Task<ShortenEntity?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera uma entrada de URL encurtada pela URL longa.
        /// Retorna null se não for encontrada.
        /// </summary>
        Task<ShortenEntity?> GetByLongUrlAsync(string longUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém o próximo ID único para uma nova URL encurtada.
        /// </summary>
        Task<long> GetNextIdAsync(CancellationToken cancellationToken = default);
    }
}