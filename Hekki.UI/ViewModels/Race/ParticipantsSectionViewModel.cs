using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.UI.Mappers;
using Hekki.UI.Messages.Race;
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

        public ObservableCollection<RaceParticipantViewModel> Participants { get; } = [];
        public ObservableCollection<PilotViewModel> FilteredPilots { get; } = [];

        public ParticipantsSectionViewModel(IRaceService raceService, IPilotService pilotService)
        {
            _raceService = raceService;
            _pilotService = pilotService;
        }

        public void Initialize(int raceId, IEnumerable<RaceParticipantViewModel> participants)
        {
            _raceId = raceId;
            Participants.Clear();
            foreach (var p in participants)
                Participants.Add(p);
        }

        partial void OnSelectedPilotChanged(PilotViewModel? value)
        {
            if (value == null) return;
            _isUpdatingFromSelection = true;
            SearchText = value.Name;
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

            if (SelectedPilot != null && SelectedPilot.Name != value)
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
                var pilots = await _pilotService.SearchPilotsByNameAsync(searchText, ct);

                if (ct.IsCancellationRequested) return;

                FilteredPilots.Clear();
                foreach (var pilot in pilots)
                    FilteredPilots.Add(new PilotViewModel { PilotId = pilot.Id, Name = pilot.Name });

                IsPopupOpen = FilteredPilots.Count > 0;
            }
            catch (OperationCanceledException) { }
        });

        [RelayCommand]
        private Task AddParticipantAsync() => ExecuteSafeAsync(async () =>
        {
            var pilot = SelectedPilot;
            if (pilot == null || _raceId == null) return;

            if (Participants.Any(p => p.PilotId == pilot.PilotId)) return;

            var participant = await _raceService.AddParticipantAsync(_raceId.Value, pilot.PilotId);
            var vm = PilotUiMapper.MapToParticipantViewModel(participant);

            Participants.Add(vm);
            SearchText = string.Empty;

            Publish(new ParticipantAddedMessage(_raceId.Value, participant));
        });

        [RelayCommand]
        private Task RemoveParticipantAsync(RaceParticipantViewModel participant) => ExecuteSafeAsync(async () =>
        {
            if (participant == null || _raceId == null) return;

            await _raceService.RemoveParticipantAsync(participant.Id);
            Participants.Remove(participant);

            Publish(new ParticipantRemovedMessage(_raceId.Value, participant.PilotId));
        });
    }
}
