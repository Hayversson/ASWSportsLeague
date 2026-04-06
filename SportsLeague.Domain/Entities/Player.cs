using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities;

public class Player : AuditBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int Number { get; set; }
    public PlayerPosition Position { get; set; }

    // Foreign Key
    public int TeamId { get; set; }

    // Navigation Property
    public Team Team { get; set; } = null!; //player has the line because is the 1 of 1*M
}