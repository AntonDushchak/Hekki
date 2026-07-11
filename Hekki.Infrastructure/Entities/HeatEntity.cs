namespace Hekki.Infrastructure.Entities
{
    public class HeatEntity
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; set; }
        public string? RoleLabel { get; set; }
        public int RegulationId { get; set; }
        public int ConfigurationIndex { get; set; }
        public int ScoringMode { get; set; }
        public RaceEntity Race { get; set; } = null!;
        public RegulationEntity Regulation { get; set; } = null!;
        public List<HeatGroupEntity> HeatGroups { get; set; } = [];
    }
}