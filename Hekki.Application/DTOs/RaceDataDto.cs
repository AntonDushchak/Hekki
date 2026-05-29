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
        public List<RaceParticipantDto> Participants { get; set; } = [];
        public List<HeatDto> Heats { get; set; } = [];
    }

    public class PilotDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Team { get; set; }
        public string? League { get; set; }
    }

    public class RaceParticipantDto
    {
        public int ParticipantId { get; set; }
        public int PilotId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public string? Team { get; set; }
        public string? League { get; set; }
        public bool IsActive { get; set; }
    }

    public class HeatDto
    {
        public int HeatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HeatNumber { get; set; }
        public int GroupCount { get; set; }
        public int ConfigurationIndex { get; set; }
        public List<HeatGroupDto> Groups { get; set; } = [];
    }

    public class HeatGroupDto
    {
        public int HeatId { get; set; }
        public int GroupIndex { get; set; }
        public int GroupNumber { get; set; }
        public int GroupCapacity { get; set; }
        public List<HeatEntryDto> Entries { get; set; } = [];
        public List<HeatResultDto> Results { get; set; } = [];
    }

    public class HeatEntryDto
    {
        public int ParticipantId { get; set; }
        public string PilotName { get; set; } = string.Empty;
        public int KartNumber { get; set; }
        public int GridPosition { get; set; }
    }

    public class HeatResultDto
    {
        public int ParticipantId { get; set; }
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
        public int? Score { get; set; }
        public int? Penalty { get; set; }
        public int TotalScore => (Score ?? 0) - (Penalty ?? 0);
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
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public RegulationConfig Config { get; set; } = new();
    }
}
