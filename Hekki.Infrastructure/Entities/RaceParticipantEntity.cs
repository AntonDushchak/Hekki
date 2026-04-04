namespace Hekki.Infrastructure.Entities
{
    public class RaceParticipantEntity
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public RaceEntity Race { get; set; } = null!;
        public int PilotId { get; set; }
        public PilotEntity Pilot { get; set; } = null!;
        public string? Team { get; set; }
        public bool IsActive { get; set; } = true;
        public List<HeatEntryEntity> HeatEntries { get; set; } = [];
        public List<HeatResultEntity> HeatResults { get; set; } = [];
    }
}