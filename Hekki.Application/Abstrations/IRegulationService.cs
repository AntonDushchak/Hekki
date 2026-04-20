using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IRegulationService
    {
        Task<IReadOnlyList<Regulation>> GetLookupAsync(CancellationToken ct = default);
    }
}
