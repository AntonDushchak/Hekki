using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.DTOs.Race;
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

            foreach (var heat in Heats)
            {
                foreach (var group in heat.Groups)
                {
                    CreateRows(group);
                    group.RefreshCells();
                }
            }
        }

        [RelayCommand]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null || _raceId == null) return;
            await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
        });

        public Task SaveCellAsync(HeatRowViewModel row, CellViewModel cell) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null || !row.HasParticipant) return;
            if (cell.Column is not HeatGroupColumn { ResultField: HeatResultField field } column) return;
            if (cell.Value is not TextCellValue text || !column.TryParse(text.Text, out var value)) return;

            var heat = Heats.FirstOrDefault(h => h.Groups.Any(g => g.Rows.Contains(row)));
            if (heat == null) return;

            try
            {
                await _raceService.SetHeatResultValueAsync(_raceId.Value, heat.HeatId, row.Entry!.ParticipantId, field, value);
            }
            catch
            {
                text.Revert();
                throw;
            }
        });

        private void CreateRows(HeatGroupViewModel heatGroup)
        {
            for (int i = heatGroup.Rows.Count; i < heatGroup.GroupCapacity; i++)
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
    }
}
