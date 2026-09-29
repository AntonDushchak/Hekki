using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.DTOs.Race;

namespace Hekki.UI.ViewModels
{
    public partial class LoadRaceViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool? _dialogResult;

        public IReadOnlyList<RaceSummaryDto> Races { get; }
        public bool HasRaces => Races.Count > 0;
        public RaceSummaryDto? Result { get; private set; }

        public LoadRaceViewModel(IReadOnlyList<RaceSummaryDto> races)
        {
            Races = races;
        }

        [RelayCommand]
        private void Select(RaceSummaryDto race)
        {
            Result = race;
            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }
    }
}
