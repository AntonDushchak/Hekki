using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Exceptions;
using Hekki.Application.Messages.Race;

namespace Hekki.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IRaceParticipantRepository _participantRepository;
        private readonly IPilotRepository _pilotRepository;
        private readonly IRegulationRepository _regulationRepository;
        private readonly IHeatRepository _heatRepository;
        private readonly IEventPublisher _eventPublisher;

        public RaceService(
            IRaceRepository raceRepository,
            IRaceParticipantRepository participantRepository,
            IPilotRepository pilotRepository,
            IRegulationRepository regulationRepository,
            IHeatRepository heatRepository,
            IEventPublisher eventPublisher)
        {
            _raceRepository = raceRepository;
            _participantRepository = participantRepository;
            _pilotRepository = pilotRepository;
            _regulationRepository = regulationRepository;
            _eventPublisher = eventPublisher;
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
            var pilot = await _pilotRepository.GetByIdAsync(pilotId, ct) ?? throw new ArgumentNullException(nameof(pilotId));
            var participant = new RaceParticipantDto { PilotId = pilot.Id, Name = pilot.Name, Team = pilot.Team, IsActive = true };
            await _participantRepository.AddAsync(participant, raceId, ct);
            _eventPublisher.Publish(new ParticipantAddedMessage(raceId, participant));
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
                    GroupNumber = i + 1,
                    GroupCapacity = config.ParticipantsPerGroup,
                    Entries = [],
                    Results = []
                });

                await _heatRepository.AddGroupAsync(heatId, groups[i], ct);
            }

            return groups;
        }

        public async Task<IReadOnlyList<GroupAssignmentResultDto>> AssignGroupsAndNumbersAsync(int raceId, int heatNumber, CancellationToken ct = default)
        {
            var race = await GetRaceDataAsync(raceId);
            var reg = await _regulationRepository.GetForEditAsync(race.RegulationId);
            var heats = await _heatRepository.GetByRaceIdAsync(raceId);
            var heat = heats.FirstOrDefault(h => h.HeatNumber == heatNumber);
            var configurationIndex = heat.ConfigurationIndex;
            var config = reg.Config.HeatConfigs[configurationIndex];
            var assignConfig = config.Assignment;
            var entries = heats.SelectMany(h => h.Groups.SelectMany(g => g.Entries)).ToList();
            var groups = heat.Groups;

            var assignedGroups = await AssignGroupsAsync(race.Participants.ToList(), assignConfig, config, groups.ToList());
            var participantsWithGroupAndKart = new List<List<ParticipantAssignmentDto>>();
            foreach (var group in assignedGroups)
            {
                participantsWithGroupAndKart.Add(AssignKartNumbersAsync(group.ToList(), assignConfig, config, entries));
            }

            if (participantsWithGroupAndKart.Count != groups.Count)
            {
                throw new InvalidOperationException("The number of assigned groups does not match the number of heat groups.");
            }
            var assignedEntries = new List<List<HeatEntryDto>>();
            foreach (var group in participantsWithGroupAndKart)
            {
                var assigmedEntry = await GenerateEntriesAsync(heat.HeatId, group.First().GroupId, group, ct);
                assignedEntries.Add(assigmedEntry.ToList());
            }

            var result = new List<GroupAssignmentResultDto>();
            for (int i = 0; i < groups.Count; i++)
            {
                result.Add(new GroupAssignmentResultDto
                {
                    Group = groups[i],
                    UpdatedEntries = assignedEntries[i]
                });
            }

            _eventPublisher.Publish(new GroupsAssignedMessage(raceId, heat.HeatId));

            return result;
        }

        public async Task<IReadOnlyList<HeatEntryDto>> GenerateEntriesAsync(int heatId, int groupId, List<ParticipantAssignmentDto> participants, CancellationToken ct = default)
        {
            var heat = await _heatRepository.GetByIdAsync(heatId, ct)
                ?? throw new HeatNotFoundException(heatId);

            var group = heat.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with ID {groupId} not found in heat {heatId}.");
            }

            if (group.GroupCapacity < participants.Count)
            {
                throw new InvalidOperationException();
            }

            var assignedEntries = new List<HeatEntryDto>();
            for (int i = 0; i < participants.Count; i++)
            {
                var assignedEntry = new HeatEntryDto
                {
                    GroupId = group.Id,
                    ParticipantId = participants[i].ParticipantId,
                    KartNumber = participants[i].KartNumber,
                    GridPosition = participants[i].GridPosition,
                };

                await _heatRepository.AddHeatEntryAsync(heatId, groupId, assignedEntry, ct);

                assignedEntries.Add(assignedEntry);
            }

            return assignedEntries;
        }


        private async Task<IReadOnlyList<IReadOnlyList<ParticipantAssignmentDto>>> AssignGroupsAsync(List<RaceParticipantDto> participants, AssignmentConfig assignConfig, HeatConfig conf, List<HeatGroupDto> groups)
        {
            var shuffled = assignConfig.Shuffle.Shuffle(participants);
            var result = assignConfig.GroupMethod.AssignGroups(shuffled, conf.ParticipantsPerGroup, conf.GroupCount);
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Count; j++)
                {
                    result[i][j] = result[i][j] with { GroupId = groups[i].Id, HeatId = groups[i].HeatId };
                }
            }
            return result;
        }

        private List<ParticipantAssignmentDto> AssignKartNumbersAsync(List<ParticipantAssignmentDto> participants, AssignmentConfig assignConfig, HeatConfig conf, List<HeatEntryDto> entries)
        {
            var avaibleKarts = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var dict = BuildKartNumbersWithPilots(participants, entries);
            var result = assignConfig.KartMethod.AssignKartNummer(dict, avaibleKarts);
            return result;
        }

        private Dictionary<ParticipantAssignmentDto, List<int>> BuildKartNumbersWithPilots(List<ParticipantAssignmentDto> participants, List<HeatEntryDto> entries)
        {
            var result = new Dictionary<ParticipantAssignmentDto, List<int>>();
            foreach (var participant in participants)
            {
                var entriesSelected = entries.Where(e => e.ParticipantId == participant.ParticipantId);
                if (entriesSelected == null || !entriesSelected.Any())
                {
                    result[participant] = new List<int>();
                    continue;
                }
                result[participant] = entriesSelected.Select(e => e.KartNumber).ToList();
            }

            return result;
        }
    }
}
