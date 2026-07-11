using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.Regulations;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private int _heatNumber = 0;

        [ObservableProperty] private HeatConfig _config = null!;

        [ObservableProperty] private ScoringMode _scoringMode;

        public bool ShowTime => ScoringMode is ScoringMode.TimeBased or ScoringMode.Hybrid;
        public bool ShowScore => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;
        public bool ShowPenalty => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;
        public ObservableCollection<HeatGroupViewModel> Groups { get; } = [];
    }

    public partial class HeatGroupViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _groupNumber = 3;

        [ObservableProperty]
        private int _groupCapacity = 8;
        [ObservableProperty]
        private int _groupIndex = 0;

        public ObservableCollection<HeatRowViewModel> Rows { get; set; } = [];
    }

    public partial class HeatRowViewModel : ObservableObject
    {
        [ObservableProperty] private HeatEntryViewModel _entry = null!;
        [ObservableProperty] private HeatResultViewModel? _result;
    }

    public partial class HeatEntryViewModel : ObservableObject
    {
        [ObservableProperty] private int _participantId;
        [ObservableProperty] private string _pilotName = string.Empty;
        [ObservableProperty] private int? _kartNumber;
        [ObservableProperty] private int? _gridPosition;
        [ObservableProperty] private int _seedOrder;
    }

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

    //public partial class Table
}
