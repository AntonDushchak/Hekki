namespace Hekki.Domain.Models
{
    public class Regulation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public int Version { get; set; }
    }
}
