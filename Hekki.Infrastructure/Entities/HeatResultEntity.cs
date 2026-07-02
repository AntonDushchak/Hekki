namespace Hekki.Infrastructure.Entities
{
    public class HeatResultEntity
    {
        public int HeatId { get; set; }
        public HeatEntity Heat { get; set; } = null!;
        public int ParticipantId { get; set; }
        public RaceParticipantEntity Participant { get; set; } = null!;
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
    }
}