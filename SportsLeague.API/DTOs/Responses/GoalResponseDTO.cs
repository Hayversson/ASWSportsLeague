using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Response;

public class GoalResponseDTO
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty; //concatenar en automapper
    public int Minute { get; set; }
    public GoalType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
