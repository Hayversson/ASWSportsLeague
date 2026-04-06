using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository : IGenericRepository<Sponsor>
    {
        Task<Sponsor?> GetByNameAsync(string name);
        Task<IEnumerable<Sponsor>> GetByTournamentIdAsync(int tournamentId);
        Task<IEnumerable<Sponsor>> GetAllWithTournamentsAsync();
        Task<Sponsor?> GetByIdWithTournamentsAsync(int id);
    }
}
