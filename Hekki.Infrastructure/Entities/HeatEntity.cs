namespace Hekki.Infrastructure.Entities
{
    public class HeatEntity
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public RaceEntity RaceEntity { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? RoleLabel { get; set; }
        public int RegulationId { get; set; }
        public RegulationEntity RegulationEntity { get; set; } = null!;
        public int ConfigurationIndex { get; set; }
        public HeatStatus Status { get; set; }
        public List<HeatEntryEntity> HeatEntries { get; set; } = [];
        public List<HeatResultEntity> HeatParticipantResults { get; set; } = [];
    }

    public enum HeatStatus
    {
        Draft,
        Locked
    }
}