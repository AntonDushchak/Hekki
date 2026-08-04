using CommunityToolkit.Mvvm.Messaging;
using Hekki.UI.Mappers;
using Hekki.UI.Messages.Race;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class TotalTableViewModel : ViewModelBase,
        IRecipient<ParticipantAddedMessage>,
        IRecipient<ParticipantRemovedMessage>,
        IRecipient<GroupsAssignedMessage>,
        IRecipient<HeatGeneratedMessage>
    {
        private int? _raceId;
        private readonly List<RaceParticipantViewModel> _participants = [];

        public ObservableCollection<HeatViewModel> Heats { get; } = [];
        public ObservableCollection<TotalTableRowViewModel> TotalTableRows { get; } = [];
        public ObservableCollection<ColumnViewModel> Columns { get; } = [];

        public void Initialize(int raceId, IEnumerable<HeatViewModel> heats, IEnumerable<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;

            Heats.Clear();
            foreach (var h in heats)
                Heats.Add(h);

            BuildColumns();

            _participants.Clear();
            _participants.AddRange(participants);

            RebuildStandings();
        }

        public void Receive(ParticipantAddedMessage message)
        {
            if (message.RaceId != _raceId) return;

            var vm = PilotUiMapper.MapToParticipantViewModel(message.RaceParticipant);
            _participants.Add(vm);
            TotalTableRows.Add(BuildRow(vm));
        }

        public void Receive(ParticipantRemovedMessage message)
        {
            if (message.RaceId != _raceId) return;

            _participants.RemoveAll(p => p.PilotId == message.ParticipantId);
            var row = TotalTableRows.FirstOrDefault(r => r.Participant.PilotId == message.ParticipantId);
            if (row != null) TotalTableRows.Remove(row);
        }

        public void Receive(GroupsAssignedMessage message)
        {
            if (message.RaceId != _raceId) return;
            RebuildStandings();
        }

        private void RebuildStandings()
        {
            TotalTableRows.Clear();
            foreach (var participant in _participants)
                TotalTableRows.Add(BuildRow(participant));
        }

        private void BuildColumns()
        {
            Columns.Clear();

            for (int i = 0; i < Heats.Count; i++)
            {
                var heat = Heats[i];
                if (heat.ShowTime)
                    Columns.Add(new ColumnViewModel
                    {
                        Type = ColumnType.HeatTime,
                        HeaderResourceKey = "m_Time",
                        Heat = heat
                    });

                if (heat.ShowScore)
                    Columns.Add(new ColumnViewModel
                    {
                        Type = ColumnType.HeatScore,
                        HeaderResourceKey = "m_Score",
                        Heat = heat
                    });
            }

            var anyTime = Heats.Any(h => h.ShowTime);
            var anyScore = Heats.Any(h => h.ShowScore);

            if (anyTime)
                Columns.Add(new ColumnViewModel { Type = ColumnType.TotalTime, HeaderResourceKey = "m_TotalTime" });
            if (anyScore)
                Columns.Add(new ColumnViewModel { Type = ColumnType.TotalScore, HeaderResourceKey = "m_TotalScore" });
        }

        private TotalTableRowViewModel BuildRow(RaceParticipantViewModel participant)
        {
            var row = new TotalTableRowViewModel(participant);
            var karts = new List<int>();

            foreach (var column in Columns)
            {
                if (column.Heat != null)
                {
                    var heat = column.Heat;
                    var rowInHeat = heat.Groups.SelectMany(g => g.Rows)
                        .FirstOrDefault(r => r.Entry.ParticipantId == participant.PilotId);

                    var result = rowInHeat?.Result;
                    if (rowInHeat?.Entry.KartNumber is int kart)
                        karts.Add(kart);

                    row.Cells.Add(new CellViewModel(column, result));
                }
                else
                {
                    long? totalTime = null;
                    int? totalScore = null;

                    foreach (var h in Heats)
                    {
                        var r = h.Groups.SelectMany(g => g.Rows)
                            .FirstOrDefault(rw => rw.Entry.ParticipantId == participant.PilotId)?.Result;
                        if (r == null) continue;
                        if (r.TotalTimeMs.HasValue)
                            totalTime = (totalTime ?? 0) + r.TotalTimeMs.Value;
                        totalScore = (totalScore ?? 0) + r.TotalScore;
                    }

                    HeatResultViewModel? aggregated = null;
                    if (column.Type == ColumnType.TotalTime)
                    {
                        aggregated = new HeatResultViewModel { TotalTimeMs = totalTime };
                    }
                    else if (column.Type == ColumnType.TotalScore)
                    {
                        aggregated = new HeatResultViewModel { Score = totalScore };
                    }

                    row.Cells.Add(new CellViewModel(column, aggregated));
                }
            }

            row.KartNumbersDisplayText = string.Join(", ", karts);
            return row;
        }

        public void Receive(HeatGeneratedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
