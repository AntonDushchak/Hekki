using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.Application.Messages.Race;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class HeatsTableViewModel : ViewModelBase,
        IRecipient<HeatGeneratedMessage>
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
            var results = await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
            UpdateGroupsAsync(results.ToList());
        });

        private void UpdateGroupsAsync(List<GroupAssignmentResultDto> results)
        {
            if (results == null || results.Count == 0) return;

            foreach (var groupResult in results)
            {
                var groupVm = Heats
                    .SelectMany(h => h.Groups)
                    .FirstOrDefault(g => g.GroupId == groupResult.Group.Id);

                if (groupVm is null)
                    continue;

                UpdateGroup(groupVm, groupResult.UpdatedEntries);
            }
        }

        private void UpdateGroup(HeatGroupViewModel groupVm, IReadOnlyList<HeatEntryDto> entries)
        {
            for (var i = 0; i < groupVm.Rows.Count; i++)
            {
                var row = groupVm.Rows[i];

                if (i < entries.Count)
                {
                    var dto = entries[i];

                    if (row.Entry is null)
                    {
                        row.Entry = Mappers.HeatUiMapper.CreateEntry(dto);
                    }
                    else
                    {
                        Mappers.HeatUiMapper.ApplyEntryAssignmentTo(dto, row.Entry);
                    }

                 
                }
                else
                {
                    row.Entry = null;
                    row.Result = null;
                }
            }
        }

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
    }
}
