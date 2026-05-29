using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IPilotRepository
    {
        Task<IReadOnlyList<PilotDto>> GetAllAsync(CancellationToken ct = default);
        Task<PilotDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(PilotDto pilot, CancellationToken ct = default);
        Task UpdateAsync(PilotDto pilot, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
