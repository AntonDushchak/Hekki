namespace Hekki.Domain.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public int HeatId { get; set; }
        public int RacePilotId { get; set; }

        public string Type { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
