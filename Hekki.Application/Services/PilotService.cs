using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public class PilotService : IPilotService
    {
        public Task<IReadOnlyList<PilotDto>> GetAllPilotsAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PilotDto?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PilotDto>> SearchPilotsByNameAsync(string searchText, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
