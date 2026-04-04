using Hekki.Domain;

namespace Hekki.Domain.Models
{
    public class HeatPilotResult
    {
        public int HeatId { get; set; }
        public int RacePilotId { get; set; }
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
        public ResultStatus Status { get; set; }
    }
}
