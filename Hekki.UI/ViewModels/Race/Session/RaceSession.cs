using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.DTOs.Race;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race.Session
{
    public partial class RaceSession : ObservableObject
    {
        [ObservableProperty] private string _raceName = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private DateTime _raceDate;

        private RaceSession(RaceDataDto race)
        {
            RaceId = race.RaceId;
            RegulationId = race.RegulationId;
            ApplyRaceInfo(race.RaceName, race.Location, race.Date);
        }

        public int RaceId { get; }
        public int RegulationId { get; }

        public ObservableCollection<ParticipantViewModel> Participants { get; } = [];
        public ObservableCollection<HeatViewModel> Heats { get; } = [];

        public HeatViewModel? LastDrawnHeat => Heats.Where(h => h.IsDrawn).MaxBy(h => h.HeatNumber);

        public static RaceSession Create(RaceDataDto race)
        {
            var session = new RaceSession(race);

            var heats = race.Heats.OrderBy(h => h.HeatNumber).ToList();
            for (var i = 0; i < heats.Count; i++)
                session.Heats.Add(CreateHeat(heats[i], i));

            foreach (var participantDto in race.Participants)
                session.AddParticipant(participantDto);

            foreach (var heat in session.Heats)
            {
                var heatDto = heats[heat.Index];
                foreach (var group in heat.Groups)
                {
                    var groupDto = heatDto.Groups.First(g => g.Id == group.GroupId);
                    foreach (var entry in groupDto.Entries)
                    {
                        var participant = session.FindParticipant(entry.ParticipantId);
                        if (participant == null) continue;

                        var row = new HeatRowViewModel(participant);
                        row.ApplyEntry(entry);
                        row.ApplyResult(groupDto.Results.FirstOrDefault(r => r.ParticipantId == entry.ParticipantId));
                        group.Rows.Add(row);
                        participant.SetHeatRow(heat.Index, row);
                    }
                    group.AddEmptySlots();
                }
            }

            return session;
        }

        public void ApplyRaceInfo(string raceName, string location, DateTime raceDate)
        {
            RaceName = raceName;
            Location = location;
            RaceDate = raceDate;
        }

        public ParticipantViewModel? FindParticipant(Guid participantId) =>
            Participants.FirstOrDefault(p => p.Id == participantId);

        public ParticipantViewModel AddParticipant(RaceParticipantDto dto)
        {
            var participant = new ParticipantViewModel(dto);
            participant.InitHeatRows(Heats.Count);
            Participants.Add(participant);
            RenumberParticipants();
            return participant;
        }

        public void UpdateParticipant(RaceParticipantDto dto)
        {
            FindParticipant(dto.ParticipantId)?.Apply(dto);
        }

        public void RemoveParticipant(Guid participantId)
        {
            var participant = FindParticipant(participantId);
            if (participant == null) return;

            Participants.Remove(participant);
            foreach (var group in Heats.SelectMany(h => h.Groups))
            {
                var row = group.Rows.FirstOrDefault(r => r.Participant == participant);
                if (row != null) group.Rows.Remove(row);
            }
            RenumberParticipants();
        }

        public void ApplyOrder(IReadOnlyList<Guid> participantIds)
        {
            Participants.ReorderBy(participantIds, p => p.Id);
            RenumberParticipants();
        }

        public void ApplyAssignment(int heatId, IReadOnlyList<GroupAssignmentResultDto> results)
        {
            var heat = Heats.FirstOrDefault(h => h.HeatId == heatId);
            if (heat == null) return;

            foreach (var groupResult in results)
            {
                var group = heat.Groups.FirstOrDefault(g => g.GroupId == groupResult.Group.Id);
                if (group == null) continue;

                var previousRows = group.Rows
                    .Where(r => r.HasParticipant)
                    .ToDictionary(r => r.Participant!.Id);

                foreach (var previous in previousRows.Values)
                    previous.Participant!.SetHeatRow(heat.Index, null);

                group.Rows.Clear();
                foreach (var entry in groupResult.UpdatedEntries)
                {
                    var participant = FindParticipant(entry.ParticipantId);
                    if (participant == null) continue;

                    if (!previousRows.TryGetValue(entry.ParticipantId, out var row))
                        row = new HeatRowViewModel(participant);

                    row.ApplyEntry(entry);
                    group.Rows.Add(row);
                    participant.SetHeatRow(heat.Index, row);
                }
            }
        }

        public void ClearAssignment(int heatId)
        {
            var heat = Heats.FirstOrDefault(h => h.HeatId == heatId);
            if (heat == null) return;

            foreach (var group in heat.Groups)
            {
                foreach (var row in group.Rows.Where(r => r.HasParticipant))
                    row.Participant!.SetHeatRow(heat.Index, null);

                group.Rows.Clear();
                group.AddEmptySlots();
            }
        }

        public void SetResult(int heatId, HeatResultDto result)
        {
            var heat = Heats.FirstOrDefault(h => h.HeatId == heatId);
            var row = heat?.Groups
                .SelectMany(g => g.Rows)
                .FirstOrDefault(r => r.Participant?.Id == result.ParticipantId);

            row?.ApplyResult(result);
        }

        private static HeatViewModel CreateHeat(HeatDto dto, int index)
        {
            var heat = new HeatViewModel(dto.HeatId, index, dto.Name, dto.HeatNumber, dto.ScoringMode);
            foreach (var groupDto in dto.Groups.OrderBy(g => g.GroupNumber))
                heat.Groups.Add(new HeatGroupViewModel(heat, groupDto.Id, groupDto.GroupNumber, groupDto.GroupCapacity));
            return heat;
        }

        private void RenumberParticipants()
        {
            for (var i = 0; i < Participants.Count; i++)
                Participants[i].Order = i + 1;
        }
    }
}
