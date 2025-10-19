using Microsoft.EntityFrameworkCore;
using PruebaEurofirms.Domain;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories;
using PruebaEurofirms.Domain.Repositories.Contract;

namespace PruebaEuroFirms.Domain.Repositories.Implementation;


/// <summary>
///   The character repository.
/// </summary>
public class CharacterRepository(EurofirmsDbContext context) : BaseRepository(context), ICharacterRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Character>> GetAllAsync(bool includeEpisodes = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Character> characters = _context.Characters;
        if (includeEpisodes)
            characters = characters.Include(c => c.Episodes);
        return await characters.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Character?> GetByApiIdAsync(int apiId, bool includeEpisodes = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Character> characters = _context.Characters;

        if (includeEpisodes)
            characters = characters.Include(c => c.Episodes);

        return await characters.FirstOrDefaultAsync(c => c.ApiId == apiId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Character>> GetByStatusAsync(string status)
    {
        return await _context.Characters
            .Where(c => c.Status.ToLower() == status.ToLower())
            .Include(c => c.Episodes)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<Character> characters)
    {
        await _context.Characters.AddRangeAsync(characters);
    }


    /// <inheritdoc/>
    public void Remove(Character character)
    {
        _context.Characters.Remove(character);
    }
}
