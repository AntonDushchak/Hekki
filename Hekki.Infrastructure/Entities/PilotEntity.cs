namespace Hekki.Infrastructure.Entities
{
    public class PilotEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public string? ProfileUrl { get; set; }
        public string? PhotoPath { get; set; }

        public List<RaceParticipantEntity> Participations { get; set; } = [];
    }
}