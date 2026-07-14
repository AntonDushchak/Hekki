using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.DTOs.Race
{
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
}
