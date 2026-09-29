namespace Hekki.Application.DTOs.Regulation
{
    public record RegulationEditDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int? OwnerRaceId { get; init; }
        public required RegulationConfig Config { get; init; }
    }
}
