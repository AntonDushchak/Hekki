using Hekki.Application.DTOs;

namespace Hekki.Application.Abstrations
{
    public interface IHeatRepository
    {
        Task<IReadOnlyList<HeatDto>> GetAllAsync(CancellationToken ct = default);
        Task<HeatDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(HeatDto heat, CancellationToken ct = default);
        Task UpdateAsync(HeatDto heat, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
