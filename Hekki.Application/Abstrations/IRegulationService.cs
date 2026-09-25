using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.Services
{
    public interface IRegulationService
    {
        Task<IReadOnlyList<RegulationSummaryDto>> GetRegulationsAsync();
        Task<RegulationEditDto> GetRegulationEditAsync(int id);
        Task<int> AddRegulationAsync(RegulationEditDto dto);
        Task DeleteRegulationAsync(int id);
    }
}