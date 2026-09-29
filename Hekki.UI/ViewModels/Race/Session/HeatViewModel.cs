using Hekki.Application.DTOs.Regulation;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels.Race.Session
{
    public class HeatViewModel
    {
        public HeatViewModel(int heatId, int index, string name, int heatNumber, ScoringMode scoringMode)
        {
            HeatId = heatId;
            Index = index;
            Name = name;
            HeatNumber = heatNumber;
            ScoringMode = scoringMode;
        }

        public int HeatId { get; }
        public int Index { get; }
        public string Name { get; }
        public int HeatNumber { get; }
        public ScoringMode ScoringMode { get; }

        public bool ShowTime => ScoringMode is ScoringMode.TimeBased or ScoringMode.Hybrid;
        public bool ShowScore => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;
        public bool ShowPenalty => ScoringMode is ScoringMode.PointsBased or ScoringMode.Hybrid;

        public ObservableCollection<HeatGroupViewModel> Groups { get; } = [];

        public int GroupCount => Groups.Count;
        public bool IsDrawn => Groups.Any(g => g.Rows.Any(r => r.HasParticipant));
        public bool HasResults => Groups.Any(g => g.Rows.Any(r => r.HasParticipant && r.HasResult));
    }
}
