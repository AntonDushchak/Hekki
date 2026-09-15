using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.Messages.Race;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class HeatsTableViewModel : ViewModelBase,
        IRecipient<HeatGeneratedMessage>,
        IRecipient<GroupsAssignedMessage>
    {
        private readonly IRaceService _raceService;
        private int? _raceId;

        public ObservableCollection<HeatViewModel> Heats { get; } = [];

        public HeatsTableViewModel(IRaceService raceService)
        {
            _raceService = raceService;
        }

        public void Initialize(int raceId, IEnumerable<HeatViewModel> heats)
        {
            _raceId = raceId;

            Heats.Clear();
            foreach (var h in heats)
                Heats.Add(h);

            foreach (var heat in Heats)
            {
                foreach (var group in heat.Groups)
                {
                    CreateRows(group);
                }
            }
        }

        [RelayCommand]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null || _raceId == null) return;
            await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
        });

        private void CreateRows(HeatGroupViewModel heatGroup)
        {
            for (int i = 0; i < heatGroup.GroupCapacity; i++)
            {
                heatGroup.Rows.Add(new HeatRowViewModel() { Entry = new HeatEntryViewModel(), Result = new HeatResultViewModel() });
            }
        }


        [RelayCommand]
        private Task EditHeatAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            await Task.CompletedTask; // TODO
        });

        [RelayCommand]
        private Task DeleteHeatAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            await Task.CompletedTask; // TODO
        });

        public void Receive(HeatGeneratedMessage message)
        {
            throw new NotImplementedException();
        }

        public void Receive(GroupsAssignedMessage message)
        {
            if (message.RaceId != _raceId)
                return;

            HeatAssignmentApplier.Apply(Heats, message.Result);
        }
    }
}
