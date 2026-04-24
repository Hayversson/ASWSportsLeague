using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class GoalRepository : GenericRepository<Goal>, IGoalRepository
{
    public GoalRepository(LeagueDbContext context) : base(context) { }

    public async Task<IEnumerable<Goal>> GetByMatchAsync(int matchId)
    {
        return await _dbSet
            .Where(g => g.MatchId == matchId)
            .OrderBy(g => g.Minute) //OrderBy ordena en forma ascendente
                                    //OrderByDescending ordena en forma descendente
            .ToListAsync();
    }

    public async Task<IEnumerable<Goal>> GetByMatchWithDetailsAsync(int matchId)
    {
        return await _dbSet
            .Where(g => g.MatchId == matchId)
            .Include(g => g.Player)
            //.ThenInclude(p => p.Team) // Incluye el equipo del jugador
            .OrderBy(g => g.Minute)
            .ToListAsync();
    }
}
