namespace Hekki.Application.DTOs.Race
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
}
