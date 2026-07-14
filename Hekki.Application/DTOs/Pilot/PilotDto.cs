namespace Hekki.Application.DTOs
{
    public record PilotDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? PhotoPath { get; init; }
        public string? ProfileUrl { get; init; }
        public string? Team { get; init; }
        public string? League { get; init; }
    }
}
