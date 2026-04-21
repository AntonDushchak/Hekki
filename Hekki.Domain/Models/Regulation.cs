namespace Hekki.Domain.Models
{
    public class Regulation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime CreationDate { get; set; }
        public List<HeatConfigurationModel> Configurations { get; set; } = [];
    }
}
