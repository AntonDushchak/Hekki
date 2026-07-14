namespace Hekki.Application.DTOs.Regulation
{
    public record RegulationConfig
    {
        public required IReadOnlyList<HeatConfig> HeatConfigs { get; init; }
    }
}
