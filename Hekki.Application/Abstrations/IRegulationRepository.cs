using Hekki.Application.DTOs.Regulation;

namespace Hekki.Application.Abstrations
{
    public interface IRegulationRepository
    {
        Task<IReadOnlyList<RegulationSummaryDto>> GetAllAsync(CancellationToken ct = default);
        Task<RegulationSummaryDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<RegulationEditDto?> GetForEditAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(RegulationEditDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
