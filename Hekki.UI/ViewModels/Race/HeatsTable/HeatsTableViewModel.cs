using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.DTOs.Race;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class HeatsTableViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IDialogService _dialogService;
        private int? _raceId;

        public ObservableCollection<HeatViewModel> Heats { get; } = [];

        public HeatsTableViewModel(IRaceService raceService, IDialogService dialogService)
        {
            _raceService = raceService;
            _dialogService = dialogService;
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
                    group.AddEmptySlots();
                    group.RefreshCells();
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanAssignGroupsAndNumbers))]
        private Task AssignGroupsAndNumbersAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (heat == null || _raceId == null) return;
            await _raceService.AssignGroupsAndNumbersAsync(_raceId.Value, heat.HeatNumber);
        });

        private static bool CanAssignGroupsAndNumbers(HeatViewModel? heat) => heat is { IsDrawn: false };

        [RelayCommand(CanExecute = nameof(CanUndoDraw))]
        private Task UndoDrawAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            var message = Localizer.Get(heat.HasResults ? "m_UndoDrawWithResultsConfirm" : "m_UndoDrawConfirm", heat.Name);
            if (!_dialogService.Confirm(Localizer.Get("m_UndoDraw"), message, Localizer.Get("m_UndoDraw"))) return;

            await _raceService.ClearHeatAssignmentAsync(_raceId.Value, heat.HeatId);
        });

        private bool CanUndoDraw(HeatViewModel? heat) => heat != null && LastDrawnHeat() == heat;

        private HeatViewModel? LastDrawnHeat() => Heats.Where(h => h.IsDrawn).MaxBy(h => h.HeatNumber);

        public void NotifyDrawStateChanged()
        {
            AssignGroupsAndNumbersCommand.NotifyCanExecuteChanged();
            UndoDrawCommand.NotifyCanExecuteChanged();
        }

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
