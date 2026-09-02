using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatResultViewModel : ObservableObject
    {
        [ObservableProperty] private int _participantId;
        [ObservableProperty] private int? _finishPosition;
        [ObservableProperty] private long? _totalTimeMs;
        [ObservableProperty] private long? _bestLapMs;
        [ObservableProperty] private int? _laps;
        [ObservableProperty] private int? _score;
        [ObservableProperty] private int? _penalty;

        public int TotalScore => (Score ?? 0) - (Penalty ?? 0);
    }
}
