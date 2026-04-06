using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
{
    public SponsorRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<Sponsor?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Sponsor>> GetByTournamentIdAsync(int tournamentId)
    {
        return await _dbSet
            .Where(s => s.TournamentSponsors.Any(ts => ts.TournamentId == tournamentId))
            .Include(s => s.TournamentSponsors)
                .ThenInclude(ts => ts.Tournament)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sponsor>> GetAllWithTournamentsAsync()
    {
        return await _dbSet
            .Include(s => s.TournamentSponsors)
                .ThenInclude(ts => ts.Tournament)
            .ToListAsync();
    }

    public async Task<Sponsor?> GetByIdWithTournamentsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.TournamentSponsors)
                .ThenInclude(ts => ts.Tournament)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
