using Hekki.Application.DTOs.Pilot;

namespace Hekki.Application.Abstrations
{
    public interface IPilotService
    {
        /// <summary>
        /// Get all pilots with basic information
        /// </summary>
        Task<IReadOnlyList<PilotDto>> GetAllPilotsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get pilot by ID with basic information
        /// </summary>
        Task<PilotDto?> GetPilotByIdAsync(int pilotId, CancellationToken ct = default);

        /// <summary>
        /// Search pilots by name
        /// </summary>
        Task<IReadOnlyList<PilotDto>> SearchPilotsByNameAsync(string searchText, CancellationToken ct = default);
    }
}
