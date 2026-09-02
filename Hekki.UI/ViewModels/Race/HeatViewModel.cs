using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.DTOs.Regulation;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatViewModel : ObservableObject
    {
        public readonly int HeatId;

        public HeatViewModel(int heatId)
        {
            HeatId = heatId;
        }

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private int _heatNumber = 0;

        [ObservableProperty] private ScoringMode _scoringMode;

        public int GroupCount => Groups.Count;
        public bool ShowTime => ScoringMode is ScoringMode.TimeBased or ScoringMode.Hybrid;
        public bool ShowScore => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;
        public bool ShowPenalty => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;
        public ObservableCollection<HeatGroupViewModel> Groups { get; } = [];
    }
}
