using Hekki.Application.Regulations;

namespace Hekki.Application.DTOs
{
    public class RaceDataDto
    {
        public int RaceId { get; set; }
        public string RaceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public int RegulationId { get; set; }
        public List<PilotDto> Participants { get; set; } = [];
        public List<HeatDto> Heats { get; set; } = [];
    }

    public class PilotDto
    {
        public int PilotId { get; set; }
        public int ParticipantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Team { get; set; }
        public List<string> KartNumbers { get; set; } = [];
        public Dictionary<string, string> Statistics { get; set; } = [];
    }

    public class HeatDto
    {
        public int HeatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; set; }
        public int GroupCount { get; set; }
        public List<HeatGroup> Groups { get; set; } = [];
    }

    public class HeatGroup
    {
        public int HeatId { get; set; }
        public int GroupNumber { get; set; }
        public int GroupCapacity { get; set; }
        public List<HeatResultDto> Results { get; set; } = [];
    }

    public class HeatResultDto
    {
        public int ParticipantId { get; set; }
        public string PilotName { get; set; } = string.Empty;
        public int? KartNumber { get; set; }
        public int? GridPosition { get; set; }
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
    }

    public class RegulationSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class RegulationEditDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public RegulationConfig Config { get; set; } = new();
    }
}
