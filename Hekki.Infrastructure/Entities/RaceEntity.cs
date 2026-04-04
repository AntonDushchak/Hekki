namespace Hekki.Infrastructure.Entities
{
    public class RaceEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DefaultReglementId { get; set; }
        public List<RaceParticipantEntity> Participants { get; set; } = [];
        public List<HeatEntity> Heats { get; set; } = [];
    }
}