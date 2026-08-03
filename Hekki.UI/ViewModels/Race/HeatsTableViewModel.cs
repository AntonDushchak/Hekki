using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Race;
using Hekki.UI.Messages.Race;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class HeatsTableViewModel : ViewModelBase
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
        }

        [RelayCommand]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null || _raceId == null) return;
            var results = await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
            await UpdateGroupsAsync(results.ToList());
            Publish(new GroupsAssignedMessage(_raceId.Value, heat.HeatId));
        });

        private Task UpdateGroupsAsync(List<GroupAssignmentResultDto> results) => ExecuteSafeAsync(async () =>
        {
            if (results == null || results.Count == 0) return;

            foreach (var groupResult in results)
            {
                var groupDto = groupResult.Group;
                var heatVm = Heats.FirstOrDefault(h => h.Groups.Any(g => g.GroupId == groupDto.Id));
                if (heatVm == null) continue;

                var groupVm = heatVm.Groups.FirstOrDefault(g => g.GroupId == groupDto.Id);
                if (groupVm == null) continue;

                // TODO: обновить groupVm из groupDto
            }

            await Task.CompletedTask;
        });

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
    }
}
