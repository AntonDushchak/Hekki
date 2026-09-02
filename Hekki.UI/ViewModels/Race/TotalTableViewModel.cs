using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Messages.Race;
using Hekki.UI.Mappers;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class TotalTableViewModel : ViewModelBase,
        IRecipient<ParticipantAddedMessage>,
        IRecipient<ParticipantRemovedMessage>,
        IRecipient<GroupsAssignedMessage>
    {
        private int? _raceId;
        private readonly List<RaceParticipantViewModel> _participants = [];

        public ObservableCollection<HeatViewModel> Heats { get; } = []; //TODO: зачем тут ваще хиты?
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
            RebuildStandingsKart();
        }

        private void RebuildStandingsHeat(int heatId)
        {
            //TotalTableRows[0].Cells[0]..Clear();
            //foreach (var participant in _participants)
            //    TotalTableRows.Add(BuildRow(participant));
        }
        private void RebuildStandingsKart()
        {
            var column = Columns
                .OfType<ParticipantColumn>()
                .FirstOrDefault();

            if (column is null)
                return;

            UpdateColumn(column);
        }

        private void UpdateColumn(ColumnViewModel column)
        {
            var columnIndex = Columns.IndexOf(column);

            if (columnIndex < 0)
                return;

            foreach (var row in TotalTableRows)
            {
                var context = new ParticipantRaceContext(row.Participant, Heats);
                column.UpdateCell(row.Cells[columnIndex], context);
            }
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

            Columns.Add(new LeagueColumn());
            //Columns.Add(new TeamColumn());
            //Columns.Add(new PhotoColumn());
            Columns.Add(new ParticipantColumn());

            foreach (var heat in Heats)
            {
                if (heat.ShowScore)
                    Columns.Add(new HeatScoreColumn(heat));

                if (heat.ShowTime)
                    Columns.Add(new HeatTimeColumn(heat));            
            }

            if (Heats.Any(h => h.ShowTime))
                Columns.Add(new TotalTimeColumn());

            if (Heats.Any(h => h.ShowScore))
                Columns.Add(new TotalScoreColumn());
        }

        private TotalTableRowViewModel BuildRow(RaceParticipantViewModel participant)
        {
            var row = new TotalTableRowViewModel(participant);

            var context = new ParticipantRaceContext(participant, Heats);

            foreach (var column in Columns)
            {
                row.Cells.Add(column.CreateCell(context));
            }

            return row;
        }
    }
}
