using PruebaEurofirms.Domain.Entities;

namespace PruebaEurofirms.Domain.Repositories.Contract;

/// <summary>
/// The episode repository interface.
/// </summary>
public interface IEpisodeRepository
{
    /// <summary>
    /// Gets all episodes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns> 
    ///   <br />
    /// </returns>
    Task<List<Episode>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple episode entries.
    /// </summary>
    /// <param name="episodes">The episodes.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    Task AddRangeAsync(IEnumerable<Episode> episodes);

    /// <summary>Saves the changes asynchronous.</summary>
    /// <returns>
    ///   <br />
    /// </returns>
    Task SaveChangesAsync();
}
