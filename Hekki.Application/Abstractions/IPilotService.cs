using Hekki.Application.DTOs.Pilot;

namespace Hekki.Application.Abstractions
{
    public interface IPilotService
    {
        Task<int> CreatePilotAsync(PilotDto pilot, CancellationToken ct = default);

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
        Task<IReadOnlyList<PilotDto>> SearchPilotsByFullNameAsync(string searchText, CancellationToken ct = default);
    }
}
