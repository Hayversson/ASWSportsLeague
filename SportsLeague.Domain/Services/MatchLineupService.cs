using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchLineupRepository matchLineupRepository,
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        ILogger<MatchLineupService> logger)
    {
        _matchLineupRepository = matchLineupRepository;
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");
        }

        if (match.Status != MatchStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Solo se pueden registrar alineaciones en partidos Scheduled");
        }

        var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
        if (player == null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el jugador con ID {lineup.PlayerId}");
        }

        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
        {
            throw new InvalidOperationException(
                "El jugador no pertenece a ninguno de los equipos del partido");
        }

        var playerAlreadyInLineup = await _matchLineupRepository
            .ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (playerAlreadyInLineup)
        {
            throw new InvalidOperationException(
                "El jugador ya está registrado en la alineación de este partido");
        }

        if (lineup.IsStarter)
        {
            var starterCount = await _matchLineupRepository
                .CountStartersByMatchAndTeamAsync(matchId, player.TeamId);
            if (starterCount >= 11)
            {
                throw new InvalidOperationException(
                    "El equipo ya tiene 11 titulares registrados en este partido");
            }
        }

        lineup.MatchId = matchId;

        _logger.LogInformation(
            "Registering lineup: Match {MatchId}, Player {PlayerId}",
            matchId, lineup.PlayerId);
        return await _matchLineupRepository.CreateAsync(lineup);
    }

    public async Task DeleteAsync(int matchId, int id)
    {
        var lineup = await _matchLineupRepository.GetByIdAsync(id);
        if (lineup == null || lineup.MatchId != matchId)
        {
            throw new KeyNotFoundException(
                $"No se encontró la entrada de alineación con ID {id} para el partido {matchId}");
        }

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");
        }

        if (match.Status != MatchStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Solo se pueden eliminar alineaciones en partidos Scheduled");
        }

        await _matchLineupRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");
        }

        _logger.LogInformation("Retrieving lineup for match {MatchId}", matchId);
        return await _matchLineupRepository.GetByMatchAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");
        }

        _logger.LogInformation(
            "Retrieving lineup for match {MatchId} and team {TeamId}",
            matchId, teamId);
        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }
}
