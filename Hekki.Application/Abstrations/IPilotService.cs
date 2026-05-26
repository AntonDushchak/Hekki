using Hekki.Domain.Models;

namespace Hekki.Application.Abstrations
{
    public interface IPilotService
    {
        /// <summary>
        /// Get all pilots with basic information
        /// </summary>
        Task<IReadOnlyList<Pilot>> GetAllPilotsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get pilot by ID with basic information
        /// </summary>
        Task<Pilot?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default);

        /// <summary>
        /// Search pilots by name
        /// </summary>
        Task<IReadOnlyList<Pilot>> SearchPilotsByNameAsync(string searchText, CancellationToken ct = default);
    }
}
