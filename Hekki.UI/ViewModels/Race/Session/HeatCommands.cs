using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.DTOs.Race;
using Hekki.Application.Exceptions;
using Hekki.UI.Mappers;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels.Race.Session
{
    public record ResultEditRequest(HeatRowViewModel Row, HeatResultField Field, long? Value);

    public partial class HeatCommands : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IDialogService _dialogService;
        private readonly IMethodCatalogService _methodCatalog;
        private readonly RaceSessionHolder _sessionHolder;

        public HeatCommands(IRaceService raceService, IDialogService dialogService, IMethodCatalogService methodCatalog, RaceSessionHolder sessionHolder)
        {
            _raceService = raceService;
            _dialogService = dialogService;
            _methodCatalog = methodCatalog;
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
            if (Session is not { } session) return;

            var config = await _raceService.GetHeatConfigAsync(session.RaceId, heat.HeatId);
            var settings = new HeatSettingsViewModel(_methodCatalog, HeatConfigUiMapper.ToViewModel(config));
            if (_dialogService.ShowHeatSettings(settings) != true) return;

            await _raceService.UpdateHeatConfigAsync(session.RaceId, heat.HeatId, HeatConfigUiMapper.ToConfig(settings.Heat, _methodCatalog));

            var race = await _raceService.GetRaceDataAsync(session.RaceId)
                ?? throw new RaceNotFoundException(session.RaceId);
            _sessionHolder.Current = RaceSession.Create(race);
            NotifyDrawStateChanged();
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
