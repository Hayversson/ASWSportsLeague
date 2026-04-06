namespace SportsLeague.Domain.Entities;

public class TournamentTeam : AuditBase
{
    public int TournamentId { get; set; } //Foreign Key
    public int TeamId { get; set; } //Foreign Key
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Tournament Tournament { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
