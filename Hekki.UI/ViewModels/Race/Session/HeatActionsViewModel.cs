using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.DTOs.Race;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels.Race.Session
{
    public record ResultEditRequest(HeatRowViewModel Row, HeatResultField Field, long? Value);

    public partial class HeatActionsViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IDialogService _dialogService;
        private readonly RaceSessionHolder _sessionHolder;

        public HeatActionsViewModel(IRaceService raceService, IDialogService dialogService, RaceSessionHolder sessionHolder)
        {
            _raceService = raceService;
            _dialogService = dialogService;
            _sessionHolder = sessionHolder;
        }

        private RaceSession? Session => _sessionHolder.Current;

        [RelayCommand(CanExecute = nameof(CanAssign))]
        private Task AssignAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var results = await _raceService.AssignGroupsAndNumbersAsync(session.RaceId, heat.HeatNumber);
            session.ApplyAssignment(heat.HeatId, results);
            NotifyDrawStateChanged();
        });

        private static bool CanAssign(HeatViewModel? heat) => heat is { IsDrawn: false };

        [RelayCommand(CanExecute = nameof(CanUndoDraw))]
        private Task UndoDrawAsync(HeatViewModel heat) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var message = Localizer.Get(heat.HasResults ? "m_UndoDrawWithResultsConfirm" : "m_UndoDrawConfirm", heat.Name);
            if (!_dialogService.Confirm(Localizer.Get("m_UndoDraw"), message, Localizer.Get("m_UndoDraw"))) return;

            await _raceService.ClearHeatAssignmentAsync(session.RaceId, heat.HeatId);
            session.ClearAssignment(heat.HeatId);
            NotifyDrawStateChanged();
        });

        private bool CanUndoDraw(HeatViewModel? heat) => heat != null && Session?.LastDrawnHeat == heat;

        [RelayCommand]
        private Task SaveResultAsync(ResultEditRequest request) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session || request.Row.Participant is not { } participant) return;

            var heat = session.FindHeatOf(request.Row);
            if (heat == null) return;

            try
            {
                var result = await _raceService.SetHeatResultValueAsync(session.RaceId, heat.HeatId, participant.Id, request.Field, request.Value);
                session.SetResult(heat.HeatId, result);
            }
            catch
            {
                request.Row.RefreshField(request.Field);
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

        public void NotifyDrawStateChanged()
        {
            AssignCommand.NotifyCanExecuteChanged();
            UndoDrawCommand.NotifyCanExecuteChanged();
        }
    }
}
