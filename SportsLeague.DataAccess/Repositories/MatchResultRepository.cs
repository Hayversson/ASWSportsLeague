using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class MatchResultRepository : GenericRepository<MatchResult>, IMatchResultRepository //Herencia y luego implementación
{
    public MatchResultRepository(LeagueDbContext context) : base(context) { }

    public async Task<MatchResult?> GetByMatchIdAsync(int matchId)
    {
        return await _dbSet
                .Where(mr => mr.MatchId == matchId)
                .FirstOrDefaultAsync();
    }
}
