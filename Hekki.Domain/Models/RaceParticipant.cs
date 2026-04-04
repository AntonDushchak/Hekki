namespace Hekki.Domain.Models
{
    public class RaceParticipant
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public int PilotId { get; set; }
        public string? Team { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
