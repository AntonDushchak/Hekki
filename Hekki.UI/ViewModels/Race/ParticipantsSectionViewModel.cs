using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race
{
    public partial class ParticipantsSectionViewModel : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;

        private int? _raceId;
        private bool _isUpdatingFromSelection;
        private CancellationTokenSource? _searchCancellation;

        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private PilotViewModel? _selectedPilot;
        [ObservableProperty] private bool _isPopupOpen;

        private IEnumerable<RaceParticipantViewModel> _participants;
        public ObservableCollection<PilotViewModel> FilteredPilots { get; } = [];

        public ParticipantsSectionViewModel(IRaceService raceService, IPilotService pilotService)
        {
            _raceService = raceService;
            _pilotService = pilotService;
            _participants = [];
        }

        public void Initialize(int raceId, IEnumerable<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;
            _participants = participants;
        }

        partial void OnSelectedPilotChanged(PilotViewModel? value)
        {
            if (value == null) return;
            _isUpdatingFromSelection = true;
            SearchText = value.FullName;
            IsPopupOpen = false;
            _isUpdatingFromSelection = false;
        }

        partial void OnSearchTextChanged(string value)
        {
            if (_isUpdatingFromSelection) return;

            if (string.IsNullOrWhiteSpace(value))
            {
                FilteredPilots.Clear();
                IsPopupOpen = false;
                return;
            }

            if (SelectedPilot != null && SelectedPilot.FullName != value)
                SelectedPilot = null;

            _ = FilterPilotsForSearchAsync(value);
        }

        private Task FilterPilotsForSearchAsync(string searchText) => ExecuteSafeAsync(async () =>
        {
            _searchCancellation?.Cancel();
            _searchCancellation = new CancellationTokenSource();
            var ct = _searchCancellation.Token;

            if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
            {
                FilteredPilots.Clear();
                IsPopupOpen = false;
                return;
            }

            try
            {
                await Task.Delay(300, ct);
                var pilots = await _pilotService.SearchPilotsByFullNameAsync(searchText, ct);

                if (ct.IsCancellationRequested) return;

                FilteredPilots.Clear();
                foreach (var pilot in pilots)
                    FilteredPilots.Add(new PilotViewModel { PilotId = pilot.Id, FirstName = pilot.FirstName, LastName = pilot.LastName });

                IsPopupOpen = FilteredPilots.Count > 0;
            }
            catch (OperationCanceledException) { }
        });

        [RelayCommand]
        private Task AddParticipantAsync(string name) => ExecuteSafeAsync(async () =>
        {
            if (string.IsNullOrEmpty(name)) return;

            if (_raceId == null) return;

            var pilot = SelectedPilot;

            if (pilot == null)
            {
                await ShowNewPilotWindow();
                return;
            }

            if (_participants.Any(p => p.PilotId == pilot.PilotId)) return;

            var participant = await _raceService.AddParticipantAsync(_raceId.Value, pilot.PilotId);

            SearchText = string.Empty;
        });

        private async Task ShowNewPilotWindow()
        {
            throw new NotImplementedException();
        }
    }
}
