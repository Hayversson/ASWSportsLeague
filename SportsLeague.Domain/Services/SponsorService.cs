using Microsoft.Extensions.Logging;
using System.Net.Mail;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllWithTournamentsAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdWithTournamentsAsync(id);

            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

            return sponsor;
        }

        public async Task<IEnumerable<Sponsor>> GetByTournamentAsync(int tournamentId)
        {
            _logger.LogInformation(
                "Retrieving sponsors for tournament ID: {TournamentId}",
                tournamentId);

            return await _sponsorRepository.GetByTournamentIdAsync(tournamentId);
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            ValidateEmail(sponsor.ContactEmail);

            var existingSponsor = await _sponsorRepository.GetByNameAsync(sponsor.Name); // Check for duplicate name
            if (existingSponsor != null)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                    $"Ya existe un sponsor con el nombre '{sponsor.Name}'");
            }

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {id}");
            }

            ValidateEmail(sponsor.ContactEmail);

            if (!string.Equals(existingSponsor.Name, sponsor.Name, StringComparison.OrdinalIgnoreCase))
            {
                var sponsorWithSameName = await _sponsorRepository.GetByNameAsync(sponsor.Name);
                if (sponsorWithSameName != null && sponsorWithSameName.Id != id)
                {
                    throw new InvalidOperationException(
                        $"Ya existe un sponsor con el nombre '{sponsor.Name}'");
                }
            }

            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.PhoneNumber = sponsor.PhoneNumber;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {id}");
            }

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");
            }

            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }

        public async Task<TournamentSponsor> LinkToTournamentAsync(
            int sponsorId,
            int tournamentId,
            decimal contractAmount)
        {
            if (contractAmount <= 0)
            {
                throw new InvalidOperationException(
                    "ContractAmount debe ser mayor a 0");
            }

            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");
            }

            var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);
            if (!tournamentExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el torneo con ID {tournamentId}");
            }

            var existingLink = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existingLink != null)
            {
                throw new InvalidOperationException(
                    "Este sponsor ya está vinculado a este torneo");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            var created = await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

            return await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(created.TournamentId, created.SponsorId)
                ?? created;
        }

        public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");
            }

            var relation = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (relation == null)
            {
                throw new KeyNotFoundException(
                    "No existe la vinculación entre el sponsor y el torneo");
            }

            await _tournamentSponsorRepository.DeleteAsync(relation.Id);
        }

        private static void ValidateEmail(string email)  //validation method for email format (ask if this is valid)
        {
            try
            {
                _ = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(
                    "ContactEmail debe tener un formato válido");
            }
        }
    }
}
