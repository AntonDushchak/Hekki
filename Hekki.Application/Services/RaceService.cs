using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Exceptions;
using Hekki.Application.Messages.Race;
using Hekki.Application.Models;

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
            var race = await GetRaceDataAsync(raceId, ct)
                ?? throw new RaceNotFoundException(raceId);
            var reg = await _regulationRepository.GetForEditAsync(race.RegulationId, ct)
                ?? throw new RegulationNotFoundException(race.RegulationId);
            var heats = await _heatRepository.GetByRaceIdAsync(raceId, ct);
            var heat = heats.FirstOrDefault(h => h.HeatNumber == heatNumber)
                ?? throw new HeatNotFoundException(heatNumber);
            var configurationIndex = heat.ConfigurationIndex;
            var config = reg.Config.HeatConfigs[configurationIndex];
            var assignConfig = config.Assignment;
            var entries = heats.SelectMany(h => h.Groups.SelectMany(g => g.Entries)).ToList();
            var groups = heat.Groups;

            var participants = race.Participants.Where(p => p.IsActive).ToList();
            var assignedGroups = AssignGroups(participants, assignConfig, config, groups.ToList());

            if (assignedGroups.Count != groups.Count)
            {
                throw new InvalidOperationException("The number of assigned groups does not match the number of heat groups.");
            }

            var result = new List<GroupAssignmentResultDto>();
            for (int i = 0; i < groups.Count; i++)
            {
                var assignedGroup = assignedGroups[i];
                AssignKartNumbers(assignedGroup, assignConfig, entries);
                var assignedEntries = await GenerateEntriesAsync(
                    heat.HeatId,
                    groups[i].Id,
                    assignedGroup,
                    participants,
                    ct);

                result.Add(new GroupAssignmentResultDto
                {
                    Group = groups[i],
                    UpdatedEntries = assignedEntries
                });
            }

            _eventPublisher.Publish(new GroupsAssignedMessage(raceId, heat.HeatId));

            return result;
        }

        private async Task<IReadOnlyList<HeatEntryDto>> GenerateEntriesAsync(
            int heatId,
            int groupId,
            IReadOnlyList<ParticipantAssignment> assignments,
            IReadOnlyList<RaceParticipantDto> participants,
            CancellationToken ct = default)
        {
            var heat = await _heatRepository.GetByIdAsync(heatId, ct)
                ?? throw new HeatNotFoundException(heatId);

            var group = heat.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with ID {groupId} not found in heat {heatId}.");
            }

            if (group.GroupCapacity < assignments.Count)
            {
                throw new InvalidOperationException();
            }

            var assignedEntries = new List<HeatEntryDto>();
            var participantNames = participants.ToDictionary(p => p.ParticipantId, p => p.Name);
            for (int i = 0; i < assignments.Count; i++)
            {
                var assignedEntry = new HeatEntryDto
                {
                    GroupId = group.Id,
                    ParticipantId = assignments[i].ParticipantId,
                    KartNumber = assignments[i].KartNumber,
                    GridPosition = assignments[i].GridPosition,
                    PilotName = participantNames[assignments[i].ParticipantId]
                };

                await _heatRepository.AddHeatEntryAsync(heatId, groupId, assignedEntry, ct);

                assignedEntries.Add(assignedEntry);
            }

            return assignedEntries;
        }


        private List<List<ParticipantAssignment>> AssignGroups(List<RaceParticipantDto> participants, AssignmentConfig assignConfig, HeatConfig conf, List<HeatGroupDto> groups)
        {
            var shuffled = assignConfig.Shuffle.Shuffle(participants);
            var result = assignConfig.GroupMethod.AssignGroups(shuffled, conf.ParticipantsPerGroup, conf.GroupCount);
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Count; j++)
                {
                    result[i][j].GroupId = groups[i].Id;
                    result[i][j].HeatId = groups[i].HeatId;
                }
            }
            return result;
        }

        private void AssignKartNumbers(IReadOnlyList<ParticipantAssignment> participants, AssignmentConfig assignConfig, List<HeatEntryDto> entries)
        {
            var availableKarts = GetAvailableKartNumbers();
            var previousKartNumbers = BuildKartNumbersWithPilots(participants, entries);
            assignConfig.KartMethod.AssignKartNummer(participants, previousKartNumbers, availableKarts);
        }

        private static IReadOnlyList<int> GetAvailableKartNumbers()
        {
            return Enumerable.Range(1, 10).ToArray();
        }

        private Dictionary<int, IReadOnlyList<int>> BuildKartNumbersWithPilots(IReadOnlyList<ParticipantAssignment> participants, List<HeatEntryDto> entries)
        {
            var result = new Dictionary<int, IReadOnlyList<int>>();
            foreach (var participant in participants)
            {
                result[participant.ParticipantId] = entries
                    .Where(e => e.ParticipantId == participant.ParticipantId)
                    .Select(e => e.KartNumber)
                    .ToList();
            }

            return result;
        }
    }
}
