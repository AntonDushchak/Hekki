namespace Hekki.Application.DTOs
{
    public record RegulationSummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Version { get; init; }
        public DateTime CreationDate { get; init; }
    }
}
