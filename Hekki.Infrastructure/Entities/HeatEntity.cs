namespace Hekki.Infrastructure.Entities
{
    public class HeatEntity
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public RaceEntity Race { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; set; }
        public string? RoleLabel { get; set; }
        public int GroupCount { get; set; }
        public int RegulationId { get; set; }
        public RegulationEntity Regulation { get; set; } = null!;
        public int ConfigurationIndex { get; set; }
        public List<HeatEntryEntity> HeatEntries { get; set; } = [];
        public List<HeatResultEntity> HeatParticipantResults { get; set; } = [];
    }
}