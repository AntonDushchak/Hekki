using Hekki.Application.Abstractions;
using Hekki.Application.DTOs.Pilot;

namespace Hekki.Application.Services
{
    public class PilotService : IPilotService
    {
        private readonly IPilotRepository _pilotRepository;

        public PilotService(IPilotRepository pilotRepository)
        {
            _pilotRepository = pilotRepository;
        }

        public async Task<int> CreatePilotAsync(PilotDto pilot, CancellationToken ct = default)
        {
            return await _pilotRepository.AddAsync(pilot, ct);
        }

        public async Task<IReadOnlyList<PilotDto>> GetAllPilotsAsync(CancellationToken ct = default)
        {
            return await _pilotRepository.GetAllAsync(ct);
        }

        public async Task<PilotDto?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default)
        {
            return await _pilotRepository.GetByIdAsync(pilotId, ct);
        }

        public async Task<IReadOnlyList<PilotDto>> SearchPilotsByFullNameAsync(string searchText, CancellationToken ct = default)
        {
            return await _pilotRepository.SearchByFullNameAsync(searchText, ct);
        }
    }
}
