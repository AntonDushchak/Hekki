using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Abstrations
{
    public interface IHeatRepository
    {
        Task<HeatDto?> GetByIdAsync(int heatId, CancellationToken ct = default);
        Task<IReadOnlyList<HeatDto>> GetByRaceIdAsync(int raceId, CancellationToken ct = default);
        Task<int> AddAsync(int raceId, HeatDto heat, CancellationToken ct = default);
        Task UpdateAsync(HeatDto heat, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<int> AddGroupAsync(int heatId, HeatGroupDto group, CancellationToken ct = default);
        Task AddHeatEntryAsync(int heatId, int groupId, HeatEntryDto entry, CancellationToken ct = default);
        Task AddHeatResultAsync(int heatId, int groupId, HeatResultDto result, CancellationToken ct = default);
        Task<int> AddHeatWithGroupsAsync(int heatId, HeatDto group, CancellationToken ct = default);
        Task AddGroupsAsync(int heatId, IReadOnlyList<HeatGroupDto> groups, CancellationToken ct = default);
    }
}
