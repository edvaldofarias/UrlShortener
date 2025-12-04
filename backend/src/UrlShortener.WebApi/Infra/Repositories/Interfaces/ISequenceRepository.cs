namespace UrlShortener.WebApi.Infra.Repositories.Interfaces;

public interface ISequenceRepository
{
    Task<long> GetNextSequenceValueAsync(string sequenceName, CancellationToken cancellationToken = default);
}