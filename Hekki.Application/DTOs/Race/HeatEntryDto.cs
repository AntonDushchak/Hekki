namespace Hekki.Application.DTOs.Race
{
    public record HeatEntryDto
    {
        public int ParticipantId { get; init; }
        public string PilotName { get; init; } = string.Empty;
        public int KartNumber { get; init; }
        public int GridPosition { get; init; }
    }
}
