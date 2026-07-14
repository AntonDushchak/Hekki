using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.DTOs
{
    public record RegulationEditDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public RegulationConfig Config { get; init; } = new();
    }
}
