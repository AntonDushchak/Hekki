namespace Hekki.Infrastructure.Entities
{
    public class RegulationEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime CreationDate { get; set; }
    }
}