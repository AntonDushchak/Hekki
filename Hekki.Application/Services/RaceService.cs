using Hekki.Application.Abstrations;
using Hekki.Application.DTOs;

namespace Hekki.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IHeatRepository _heatRepository;
        private readonly IRegulationRepository _regulationRepository;
        private readonly IRaceParticipantRepository _raceParticipantRepository;
        private readonly IHeatResultRepository _heatResultRepository;
        private readonly IHeatEntryRepository _heatEntryRepository;

        public RaceService(
            IRaceRepository raceRepository,
            IHeatRepository heatRepository,
            IRegulationRepository regulationRepository,
            IRaceParticipantRepository raceParticipantRepository,
            IHeatResultRepository heatResultRepository,
            IHeatEntryRepository heatEntryRepository)
        {
            _raceRepository = raceRepository;
            _heatRepository = heatRepository;
            _regulationRepository = regulationRepository;
            _raceParticipantRepository = raceParticipantRepository;
            _heatResultRepository = heatResultRepository;
            _heatEntryRepository = heatEntryRepository;
        }

        public Task<int> AddParticipantAsync(int raceId, int pilotId, string team, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddPilotToRaceAsync(int raceId, int pilotId, string? team = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<HeatGroupDto>> GetHeatGroupsAsync(int heatId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<HeatResultDto>> GetHeatResultsAsync(int heatId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PilotDto?> GetRaceByIdAsync(int raceId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<RaceDataDto> GetRaceDataAsync(int raceId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<HeatDto>> GetRaceHeatsAsync(int raceId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PilotDto>> GetRaceParticipantsAsync(int raceId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<RegulationEditDto?> GetRaceRegulationAsync(int regulationId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsParticipantInRaceAsync(int raceId, int pilotId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveParticipantAsync(int participantId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveParticipantFromRaceAsync(int raceId, int participantId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PilotDto>> SearchPilotsAsync(int raceId, string searchText, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
