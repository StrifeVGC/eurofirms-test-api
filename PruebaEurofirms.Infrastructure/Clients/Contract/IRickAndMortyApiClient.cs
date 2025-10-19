using PruebaEurofirms.Infrastructure.Models.DTO;

/// <summary>
/// 
/// </summary>
public interface IRickAndMortyApiClient
{
    /// <summary>
    /// Gets all characters asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<List<RickAndMortyCharacterDto>> GetAllCharactersAsync(CancellationToken cancellationToken = default);
}