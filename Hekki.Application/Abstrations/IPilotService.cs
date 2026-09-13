using Hekki.Application.DTOs.Pilot;

namespace Hekki.Application.Abstrations
{
    public interface IPilotService
    {
        void CreatePilot(string firstName, string lastName);

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
