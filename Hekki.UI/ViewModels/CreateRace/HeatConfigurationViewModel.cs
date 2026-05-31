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


        [ObservableProperty] private string _shuffleMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _shuffleParameters;

        [ObservableProperty] private string _groupMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _groupParameters;

        [ObservableProperty] private string _kartMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _kartParameters;

        [ObservableProperty] private string _scoreMethodId = string.Empty;
        [ObservableProperty] private MethodParameters? _scoreParameters;

    }
}
