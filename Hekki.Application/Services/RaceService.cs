using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Exceptions;

namespace Hekki.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IRaceParticipantRepository _participantRepository;
        private readonly IPilotRepository _pilotRepository;
        private readonly IRegulationRepository _regulationRepository;
        private readonly IHeatRepository _heatRepository;

        public RaceService(
            IRaceRepository raceRepository,
            IRaceParticipantRepository participantRepository,
            IPilotRepository pilotRepository,
            IRegulationRepository regulationRepository,
            IHeatRepository heatRepository)
        {
            _raceRepository = raceRepository;
            _participantRepository = participantRepository;
            _pilotRepository = pilotRepository;
            _regulationRepository = regulationRepository;
            _heatRepository = heatRepository;
        }

        public async Task<RaceDataDto?> GetRaceDataAsync(int raceId, CancellationToken ct = default)
        {
            return await _raceRepository.GetByIdAsync(raceId, ct);
        }

        public async Task<int> CreateRaceAsync(string name, string location, DateTime date, int regulationId, CancellationToken ct = default)
        {
            var utcDate = date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);
            var race = new RaceDataDto { RaceName = name, Location = location, Date = utcDate, RegulationId = regulationId, Heats = [], Participants = [] };
            var regulation = await _regulationRepository.GetByIdAsync(regulationId, ct);
            return await _raceRepository.AddAsync(race, ct);
        }

        public async Task<RaceParticipantDto> AddParticipantAsync(int raceId, int pilotId, CancellationToken ct = default)
        {
            var pilot = await _pilotRepository.GetByIdAsync(pilotId, ct);
            var participant = new RaceParticipantDto { PilotId = pilot.Id, Name = pilot.Name, Team = pilot.Team, IsActive = true };
            await _participantRepository.AddAsync(participant, raceId, ct);
            return participant;
        }

        public async Task RemoveParticipantAsync(int participantId, CancellationToken ct = default)
        {
            await _participantRepository.DeleteAsync(participantId, ct);
        }

        public async Task<RegulationEditDto?> GetRegulationEditAsync(int regulationId, CancellationToken ct = default)
        {
            return await _regulationRepository.GetForEditAsync(regulationId, ct);
        }

        public async Task<IReadOnlyList<HeatDto>> GenerateHeatsWithGroupsAsync(int raceId, CancellationToken ct = default)
        {
            var heats = await GenerateHeatsAsync(raceId, ct);

            var heatsWithGroups = new List<HeatDto>();
            foreach (var heat in heats)
            {
                var groups = await GenerateGroupsAsync(raceId, heat.HeatId, ct);
                heatsWithGroups.Add(heat with { Groups = groups, GroupCount = groups.Count });
            }

            return heatsWithGroups;
        }

        public async Task<IReadOnlyList<HeatDto>> GenerateHeatsAsync(int raceId, CancellationToken ct = default)
        {
            var race = await _raceRepository.GetByIdAsync(raceId, ct)
                ?? throw new RaceNotFoundException(raceId);

            var regulation = await _regulationRepository.GetForEditAsync(race.RegulationId, ct)
                ?? throw new RegulationNotFoundException(race.RegulationId);

            var config = regulation.Config;

            var heats = new List<HeatDto>();
            for (int configIndex = 0; configIndex < config.HeatConfigs.Count; configIndex++)
            {
                var heatDto = new HeatDto
                {
                    RaceId = raceId,
                    Name = config.HeatConfigs[configIndex].Name,
                    HeatNumber = config.HeatConfigs[configIndex].HeatNumber,
                    ConfigurationIndex = configIndex,
                    ScoringMode = config.HeatConfigs[configIndex].ScoringMode,
                    RegulationId = regulation.Id,
                    Groups = []
                };

                var heatId = await _heatRepository.AddAsync(raceId, heatDto, ct);
                heats.Add(heatDto with { HeatId = heatId });
            }

            return heats;
        }

        public async Task<IReadOnlyList<HeatGroupDto>> GenerateGroupsAsync(int raceId, int heatId, CancellationToken ct = default)
        {
            var race = await _raceRepository.GetByIdAsync(raceId, ct)
                ?? throw new RaceNotFoundException(raceId);
            var regulation = await _regulationRepository.GetForEditAsync(race.RegulationId, ct)
                ?? throw new RegulationNotFoundException(race.RegulationId);
            var heat = await _heatRepository.GetByIdAsync(heatId, ct)
                ?? throw new HeatNotFoundException(heatId);

            var config = regulation.Config.HeatConfigs[heat.ConfigurationIndex];

            var groups = new List<HeatGroupDto>();
            for (int i = 0; i < config.GroupCount; i++)
            {
                groups.Add(new HeatGroupDto
                {
                    HeatId = heatId,
                    GroupIndex = i,
                    GroupNumber = i + 1,
                    GroupCapacity = config.ParticipantsPerGroup,
                    Entries = [],
                    Results = []
                });

                await _heatRepository.AddGroupAsync(heatId, groups[i], ct);
            }

            return groups;
        }
    }
}
