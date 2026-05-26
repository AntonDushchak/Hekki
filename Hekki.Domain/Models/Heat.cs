using Hekki.Domain;

namespace Hekki.Domain.Models
{
    public class Heat
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? RoleLabel { get; set; }
        public int RegulationId { get; set; }
        public int ConfigurationIndex { get; set; }
        public HeatStatus Status { get; set; }
    }
}
