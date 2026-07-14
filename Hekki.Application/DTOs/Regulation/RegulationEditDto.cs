namespace Hekki.Application.DTOs.Regulation
{
    public record RegulationEditDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public required RegulationConfig Config { get; init; }
    }
}
