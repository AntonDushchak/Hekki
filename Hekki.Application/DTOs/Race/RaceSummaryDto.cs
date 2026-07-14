namespace Hekki.Application.DTOs.Race
{
    public record RaceSummaryDto
    {
        public int RaceId { get; init; }
        public string RaceName { get; init; } = string.Empty;
        public DateTime Date { get; init; }
        public string Location { get; init; } = string.Empty;
        public int RegulationId { get; init; }
    }
}
