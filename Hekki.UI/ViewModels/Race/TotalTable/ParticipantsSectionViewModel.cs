using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class ParticipantsSectionViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private readonly IDialogService _dialogService;
        private readonly AppSettings _appSettings;
        private int? _raceId;
        private CancellationTokenSource? _searchCancellation;

        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private bool _isPopupOpen;

        private IReadOnlyCollection<RaceParticipantViewModel> _participants;
        public ObservableCollection<object> Suggestions { get; } = [];

        public ParticipantsSectionViewModel(
            IRaceService raceService,
            IPilotService pilotService,
            IDialogService dialogService,
            AppSettings appSettings)
        {
            _raceService = raceService;
            _pilotService = pilotService;
            _dialogService = dialogService;
            _appSettings = appSettings;
            _participants = [];
        }

        public void Initialize(int raceId, IReadOnlyCollection<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;
            _participants = participants;
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _searchCancellation?.Cancel();
                Suggestions.Clear();
                IsPopupOpen = false;
                return;
            }

            _ = FilterPilotsForSearchAsync(value);
        }

        private Task FilterPilotsForSearchAsync(string searchText) => ExecuteSafeAsync(async () =>
        {
            _searchCancellation?.Cancel();
            _searchCancellation = new CancellationTokenSource();
            var ct = _searchCancellation.Token;

            if (searchText.Trim().Length < 3)
            {
                Suggestions.Clear();
                IsPopupOpen = false;
                return;
            }

            await Task.Delay(300, ct);
            var pilots = await _pilotService.SearchPilotsByFullNameAsync(searchText, ct);

            ct.ThrowIfCancellationRequested();

            Suggestions.Clear();
            foreach (var pilot in pilots.Where(p => !IsInRace(p.Id)))
                Suggestions.Add(new PilotViewModel { PilotId = pilot.Id, FirstName = pilot.FirstName, LastName = pilot.LastName });
            Suggestions.Add(new NewPilotSuggestion(searchText.Trim()));

            IsPopupOpen = true;
        });

        [RelayCommand]
        private Task SelectSuggestionAsync(object? suggestion) => ExecuteSafeAsync(async () =>
        {
            if (_raceId == null) return;

            IsPopupOpen = false;

            var pilotId = suggestion switch
            {
                PilotViewModel pilot => pilot.PilotId,
                NewPilotSuggestion newPilot => await CreatePilotAsync(newPilot.FullName),
                _ => null
            };

            if (pilotId == null || IsInRace(pilotId.Value)) return;

            await _raceService.AddParticipantAsync(_raceId.Value, pilotId.Value);

            SearchText = string.Empty;
        });

        private async Task<int?> CreatePilotAsync(string fullName)
        {
            var names = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var editorViewModel = new PilotEditorViewModel(_appSettings)
            {
                FirstName = names.ElementAtOrDefault(0) ?? string.Empty,
                LastName = names.ElementAtOrDefault(1) ?? string.Empty
            };

            var pilot = _dialogService.ShowPilotEditor(editorViewModel);
            if (pilot == null) return null;

            return await _pilotService.CreatePilotAsync(pilot);
        }

        private bool IsInRace(int pilotId) => _participants.Any(p => p.PilotId == pilotId);
    }

    public record NewPilotSuggestion(string FullName);
}
