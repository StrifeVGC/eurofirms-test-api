using PruebaEurofirms.Domain.Entities;

namespace PruebaEurofirms.Domain.Repositories.Contract;

/// <summary>
/// The character repository interface.
/// </summary>
public interface ICharacterRepository
{
    /// <summary>Gets all asynchronous.</summary>
    /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    Task<IEnumerable<Character>> GetAllAsync(bool includeEpisodes = false, CancellationToken cancellationToken = default);


    /// <summary>
    /// Gets the by the ApiId asynchronous.
    /// </summary>
    /// <param name="apiId">The API identifier.</param>
    /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Character?> GetByApiIdAsync(int apiId, bool includeEpisodes = false, CancellationToken cancellationToken = default);


    /// <summary>Gets the by status asynchronous.</summary>
    /// <param name="status">The status.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    Task<List<Character>> GetByStatusAsync(string status);

    /// <summary>Adds the range asynchronous.</summary>
    /// <param name="characters">The characters.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    Task AddRangeAsync(IEnumerable<Character> characters);

    /// <summary>Removes the character.</summary>
    /// <param name="character">The character.</param>
    void Remove(Character character);

    /// <summary>Saves the changes asynchronous.</summary>
    /// <returns>
    ///   <br />
    /// </returns>
    Task SaveChangesAsync();
}
