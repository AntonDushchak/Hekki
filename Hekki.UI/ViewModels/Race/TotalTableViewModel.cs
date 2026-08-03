using CommunityToolkit.Mvvm.Messaging;
using Hekki.UI.Mappers;
using Hekki.UI.Messages.Race;
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

        public ObservableCollection<HeatViewModel> Heats { get; } = [];
        public ObservableCollection<TotalTableRowViewModel> TotalTableRows { get; } = [];

        public void Initialize(int raceId, IEnumerable<HeatViewModel> heats, IEnumerable<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;

            Heats.Clear();
            foreach (var h in heats)
                Heats.Add(h);

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

        private TotalTableRowViewModel BuildRow(RaceParticipantViewModel participant)
        {
            var row = new TotalTableRowViewModel(participant);
            var karts = new List<int>();

            foreach (var heat in Heats)
            {
                var rowInHeat = heat.Groups.SelectMany(g => g.Rows)
                    .FirstOrDefault(r => r.Entry.ParticipantId == participant.PilotId);

                row.HeatCells.Add(new HeatResultCellViewModel(heat, rowInHeat?.Result));

                if (rowInHeat?.Entry.KartNumber is int kart)
                    karts.Add(kart);
            }

            row.KartNumbersDisplayText = string.Join(", ", karts);
            return row;
        }
    }
}
