using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.Regulations;

namespace Hekki.UI.ViewModels
{
    public partial class HeatConfigurationViewModel : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private int _heatNumber;
        [ObservableProperty] private int _numberOfGroups;
        [ObservableProperty] private int _groupCapacity;
        [ObservableProperty] private bool _usePenalty;
        [ObservableProperty] private ScoringMode _scoringMode;

        [ObservableProperty] private bool _usePoints;
        [ObservableProperty] private bool _useTime;


        [ObservableProperty] private string _shuffleMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _shuffleParameters;

        [ObservableProperty] private string _groupMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _groupParameters;

        [ObservableProperty] private string _kartMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _kartParameters;

        [ObservableProperty] private string _scoreMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _scoreParameters;

        partial void OnUsePointsChanged(bool value)
        {
            UpdateScoringMode();
        }

        partial void OnUseTimeChanged(bool value)
        {
            UpdateScoringMode();
        }

        partial void OnScoringModeChanged(ScoringMode value)
        {
            UsePoints = value is ScoringMode.PointsBased or ScoringMode.Hybrid;
            UseTime = value is ScoringMode.TimeBased or ScoringMode.Hybrid;
        }

        private void UpdateScoringMode()
        {
            if (UsePoints && UseTime)
            {
                ScoringMode = ScoringMode.Hybrid;
            }
            else if (UsePoints)
            {
                ScoringMode = ScoringMode.PointsBased;
            }
            else if (UseTime)
            {
                ScoringMode = ScoringMode.TimeBased;
            }
        }

        public bool IsValid()
        {
            return UsePoints || UseTime;
        }
    }
}
