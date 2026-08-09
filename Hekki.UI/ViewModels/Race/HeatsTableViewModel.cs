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
        }

        [RelayCommand]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null || _raceId == null) return;
            var results = await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
            await UpdateGroupsAsync(results.ToList()); //TODO
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

                foreach (var entryDto in groupResult.UpdatedEntries)
                {
                    var entryVm = groupVm.Rows
                        .Select(r => r.Entry)
                        .FirstOrDefault(e => e.ParticipantId == entryDto.ParticipantId);
                    if (entryVm == null)
                    {
                        throw new NotImplementedException(); //TODO
                    }
                    Mappers.HeatUiMapper.ApplyEntryAssignmentTo(entryDto, entryVm);
                }
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

        public void Receive(HeatGeneratedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
