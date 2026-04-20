using Hekki.Application.Abstrations;
using Hekki.Domain.Models;

namespace Hekki.Infrastructure
{
    public class RegulationService : IRegulationService
    {
        private readonly IRegulationRepository _regulationRepository;

        public RegulationService(IRegulationRepository regulationRepository)
        {
            _regulationRepository = regulationRepository;
        }

        public Task<IReadOnlyList<Regulation>> GetLookupAsync(CancellationToken ct = default)
        {
            return _regulationRepository.GetLookupAsync(ct);
        }
    }
}