namespace Hekki.Application.DTOs.Race
{
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
}
