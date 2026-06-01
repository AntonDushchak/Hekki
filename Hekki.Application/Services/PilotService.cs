using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public class PilotService : IPilotService
    {
        private readonly IPilotRepository _pilotRepository;

        public PilotService(IPilotRepository pilotRepository)
        {
            _pilotRepository = pilotRepository;
        }

        public async Task<IReadOnlyList<PilotDto>> GetAllPilotsAsync(CancellationToken ct = default)
        {
            return await _pilotRepository.GetAllAsync(ct);
        }

        public async Task<PilotDto?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default)
        {
            return await _pilotRepository.GetByIdAsync(pilotId, ct);
        }

        public async Task<IReadOnlyList<PilotDto>> SearchPilotsByNameAsync(string searchText, CancellationToken ct = default)
        {
            return await _pilotRepository.SearchByNameAsync(searchText, ct);
        }
    }
}
