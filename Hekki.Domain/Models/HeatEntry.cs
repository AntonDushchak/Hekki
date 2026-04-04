namespace Hekki.Domain.Models
{
    public class HeatEntry
    {
        public int HeatId { get; set; }
        public int RacePilotId { get; set; }

        public int SeedOrder { get; set; }
        public int? GridPosition { get; set; }
        public int? KartNumber { get; set; }
    }
}
