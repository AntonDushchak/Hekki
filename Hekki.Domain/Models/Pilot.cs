namespace Hekki.Domain.Models
{
    public class Pilot
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileUrl { get; set; }
        public string? PhotoPath { get; set; }
    }
}
