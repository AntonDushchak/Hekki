using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.Services
{
    public class RegulationService : IRegulationService
    {
        private readonly IRegulationRepository _regulationRepository;

        public RegulationService(IRegulationRepository regulationRepository)
        {
            _regulationRepository = regulationRepository;            
        }

        public async Task DeleteRegulationAsync(int id)
        {
            await _regulationRepository.DeleteAsync(id);
        }

        public async Task<RegulationEditDto> GetRegulationEditAsync(int id)
        {
            return await _regulationRepository.GetForEditAsync(id);
        }

        public async Task<IReadOnlyList<RegulationSummaryDto>> GetRegulationsAsync()
        {
            return await _regulationRepository.GetAllAsync();
        }
    }
}
