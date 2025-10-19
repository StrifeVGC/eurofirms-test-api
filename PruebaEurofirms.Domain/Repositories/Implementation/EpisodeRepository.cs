using Microsoft.EntityFrameworkCore;
using PruebaEurofirms.Domain;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories;
using PruebaEurofirms.Domain.Repositories.Contract;

namespace PruebaEuroFirms.Domain.Repositories.Implementation;

public class EpisodeRepository(EurofirmsDbContext context) : BaseRepository(context), IEpisodeRepository
{
    public async Task<List<Episode>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Episodes.ToListAsync(cancellationToken);
    }

    public async Task<Episode?> GetByIdAsync(int ApiId)
    {
        return await _context.Episodes.FirstOrDefaultAsync(e => e.ApiId == ApiId);
    }

    public async Task<List<Episode>> GetByIdsAsync(IEnumerable<int> ApiIds)
    {
        return await _context.Episodes.Where(e => ApiIds.Contains(e.ApiId)).ToListAsync();
    }

    public async Task AddAsync(Episode episode)
    {
        await _context.Episodes.AddAsync(episode);
    }

    public async Task AddRangeAsync(IEnumerable<Episode> episodes)
    {
        await _context.Episodes.AddRangeAsync(episodes);
    }

    public void RemoveAsync(Episode episode)
    {
        _context.Episodes.Remove(episode);
    }

    public async Task<List<Episode>> GetOrphanEpisodesAsync()
    {
        return await _context.Episodes
            .Where(e => !e.Characters.Any())
            .ToListAsync();
    }
}
