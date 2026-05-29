using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public interface IRegulationService
    {
        Task<List<RegulationSummaryDto>> GetRegulationsAsync();
        Task<RegulationEditDto> GetRegulationEditAsync(int id);
    }
}