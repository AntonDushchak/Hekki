namespace Hekki.Infrastructure.Entities
{
    public class HeatEntryEntity
    {
        public int HeatId { get; set; }
        public HeatEntity Heat { get; set; } = null!;
        public int ParticipantId { get; set; }
        public RaceParticipantEntity Participant { get; set; } = null!;

        public int SeedOrder { get; set; }
        public int? GridPosition { get; set; }
        public int? KartNumber { get; set; }
    }
}