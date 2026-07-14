using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Abstrations
{
    public interface IRaceRepository
    {
        Task<IReadOnlyList<RaceDataDto>> GetAllAsync(CancellationToken ct = default);
        Task<RaceDataDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(RaceDataDto race, CancellationToken ct = default);
        Task UpdateAsync(RaceDataDto race, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
