namespace Hekki.Infrastructure.Entities
{
    public class HeatEntryEntity
    {
        public int GroupId { get; set; }
        public HeatGroupEntity Group { get; set; } = null!;
        public int ParticipantId { get; set; }
        public RaceParticipantEntity Participant { get; set; } = null!;
        public int? KartNumber { get; set; }
        public int SeedOrder { get; set; }
        public int? GridPosition { get; set; }
    }
}