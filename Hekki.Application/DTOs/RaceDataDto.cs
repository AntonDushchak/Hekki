using Hekki.Application.Regulations;

namespace Hekki.Application.DTOs
{
    public record RaceDataDto
    {
        public int RaceId { get; init; }
        public string RaceName { get; init; } = string.Empty;
        public DateTime Date { get; init; }
        public string Location { get; init; } = string.Empty;
        public int RegulationId { get; init; }
        public IReadOnlyList<RaceParticipantDto> Participants { get; init; }
        public IReadOnlyList<HeatDto> Heats { get; init; }
    }

    public record RaceSummaryDto
    {
        public int RaceId { get; init; }
        public string RaceName { get; init; } = string.Empty;
        public DateTime Date { get; init; }
        public string Location { get; init; } = string.Empty;
        public int RegulationId { get; init; }
    }

    public record PilotDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? PhotoPath { get; init; }
        public string? ProfileUrl { get; init; }
        public string? Team { get; init; }
        public string? League { get; init; }
    }

    public record RaceParticipantDto
    {
        public int ParticipantId { get; init; }
        public int PilotId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? PhotoPath { get; init; }
        public string? Team { get; init; }
        public string? League { get; init; }
        public bool IsActive { get; init; }
    }

    public record HeatDto
    {
        public int HeatId { get; init; }
        public int RaceId { get; init; }
        public string Name { get; init; } = string.Empty;
        public int HeatNumber { get; init; }
        public int GroupCount { get; init; }
        public int ConfigurationIndex { get; init; }
        public int RegulationId { get; init; }
        public ScoringMode ScoringMode { get; init; }
        public IReadOnlyList<HeatGroupDto> Groups { get; init; } = [];
    }

    public record HeatGroupDto
    {
        public int HeatId { get; init; }
        public int GroupIndex { get; init; }
        public int GroupNumber { get; init; }
        public int GroupCapacity { get; init; }
        public IReadOnlyList<HeatEntryDto> Entries { get; init; } = [];
        public IReadOnlyList<HeatResultDto> Results { get; init; } = [];
    }

    public record HeatEntryDto
    {
        public int ParticipantId { get; init; }
        public string PilotName { get; init; } = string.Empty;
        public int KartNumber { get; init; }
        public int GridPosition { get; init; }
    }

    public record HeatResultDto
    {
        public int ParticipantId { get; init; }
        public int? FinishPosition { get; init; }
        public long? TotalTimeMs { get; init; }
        public long? BestLapMs { get; init; }
        public int? Laps { get; init; }
        public int? Score { get; init; }
        public int? Penalty { get; init; }
        public int TotalScore => (Score ?? 0) - (Penalty ?? 0);
    }

    public record RegulationSummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Version { get; init; }
        public DateTime CreationDate { get; init; }
    }

    public record RegulationEditDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public RegulationConfig Config { get; init; } = new();
    }
}
