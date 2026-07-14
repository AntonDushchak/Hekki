namespace Hekki.Application.DTOs.Regulation
{
    public record RegulationSummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Version { get; init; }
        public DateTime CreationDate { get; init; }
    }
}
