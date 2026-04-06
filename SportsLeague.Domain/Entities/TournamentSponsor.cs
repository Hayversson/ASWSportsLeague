namespace SportsLeague.Domain.Entities
{
    public class TournamentSponsor : AuditBase
    {
        public int TournamentId { get; set; } //Foreign Key
        public int SponsorId { get; set; } //Foreign Key
        public decimal ContractAmount { get; set; }
        public DateTime JoinedAt { get; set; }

        // Navigation Properties
        public Tournament Tournament { get; set; } = null!;
        public Sponsor Sponsor { get; set; } = null!;
    }
}
