using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public interface IRegulationService
    {
        Task<IReadOnlyList<RegulationSummaryDto>> GetRegulationsAsync();
        Task<RegulationEditDto> GetRegulationEditAsync(int id);
    }
}