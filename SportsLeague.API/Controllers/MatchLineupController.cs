using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Responses;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[Route("api/match/{matchId}/lineup")]
[ApiController]
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _matchLineupService;
    private readonly IMapper _mapper;

    public MatchLineupController(
        IMatchLineupService matchLineupService,
        IMapper mapper)
    {
        _matchLineupService = matchLineupService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MatchLineupDTO>> RegisterLineup(
        int matchId, CreateMatchLineupDTO lineupDTO)
    {
        try
        {
            var lineup = _mapper.Map<MatchLineup>(lineupDTO);
            var created = await _matchLineupService.AddPlayerAsync(matchId, lineup);
            var lineups = await _matchLineupService.GetByMatchAsync(matchId);
            var createdLineup = lineups.FirstOrDefault(l => l.Id == created.Id);
            return Ok(_mapper.Map<MatchLineupDTO>(createdLineup));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchLineupDTO>>> GetLineup(int matchId)
    {
        try
        {
            var lineups = await _matchLineupService.GetByMatchAsync(matchId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDTO>>(lineups));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<IEnumerable<MatchLineupDTO>>> GetLineupByTeam(
        int matchId, int teamId)
    {
        try
        {
            var lineups = await _matchLineupService.GetByMatchAndTeamAsync(matchId, teamId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDTO>>(lineups));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLineup(int matchId, int id)
    {
        try { await _matchLineupService.DeleteAsync(matchId, id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}
