using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(LeagueDbContext context) : base(context)
        {
        }
        public async Task<Team?> GetByNameAsync(string name) //Here i return and object TEAM type
        {
            return await _dbSet
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }
        public async Task<IEnumerable<Team>> GetByCityAsync(string city) //Here i return a list of objetcs of TEAM type
        {
            return await _dbSet
            .Where(t => t.City.ToLower() == city.ToLower())
            .ToListAsync();
        }
    }

}
