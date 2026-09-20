using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class MethodSettingsEditorViewModel : ObservableObject
    {
        private readonly Dictionary<MethodSettingsType, MethodConfiguration> _methodConfigurations;
        [ObservableProperty] private HeatConfigurationViewModel? _selectedHeat;
        [ObservableProperty] private MethodParameters? _currentActiveSettings;
        [ObservableProperty] private string? _currentActiveSettingsTitle;
        [ObservableProperty] private MethodSettingsType? _currentSettingsType;

        public ObservableCollection<MethodOption> AvailableShuffleMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableGroupMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableKartMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableScoreMethods { get; } = [];

        public bool IsShuffleActive => CurrentSettingsType == MethodSettingsType.Shuffle;
        public bool IsGroupActive => CurrentSettingsType == MethodSettingsType.Group;
        public bool IsKartActive => CurrentSettingsType == MethodSettingsType.Kart;
        public bool IsScoreActive => CurrentSettingsType == MethodSettingsType.Score;

        public MethodSettingsEditorViewModel(IMethodCatalogService methodCatalog)
        {
            foreach (var option in methodCatalog.GetShuffleOptions())
                AvailableShuffleMethods.Add(option);

            foreach (var option in methodCatalog.GetGroupOptions())
                AvailableGroupMethods.Add(option);

            foreach (var option in methodCatalog.GetKartOptions())
                AvailableKartMethods.Add(option);

            foreach (var option in methodCatalog.GetScoreOptions())
                AvailableScoreMethods.Add(option);

            _methodConfigurations = new()
            {
                [MethodSettingsType.Shuffle] = new(
                    () => SelectedHeat?.ShuffleMethodId,
                    () => SelectedHeat?.ShuffleParameters,
                    AvailableShuffleMethods),
                [MethodSettingsType.Group] = new(
                    () => SelectedHeat?.GroupMethodId,
                    () => SelectedHeat?.GroupParameters,
                    AvailableGroupMethods),
                [MethodSettingsType.Kart] = new(
                    () => SelectedHeat?.KartMethodId,
                    () => SelectedHeat?.KartParameters,
                    AvailableKartMethods),
                [MethodSettingsType.Score] = new(
                    () => SelectedHeat?.ScoreMethodId,
                    () => SelectedHeat?.ScoreParameters,
                    AvailableScoreMethods)
            };
        }

        partial void OnSelectedHeatChanged(HeatConfigurationViewModel? value)
        {
            if (value == null)
            {
                ResetMethodSettingsState();
                return;
            }
        }

        private void ResetMethodSettingsState()
        {
            CurrentActiveSettings = null;
            CurrentActiveSettingsTitle = null;
            CurrentSettingsType = null;
        }

        partial void OnCurrentSettingsTypeChanged(MethodSettingsType? value)
        {
            OnPropertyChanged(nameof(IsShuffleActive));
            OnPropertyChanged(nameof(IsGroupActive));
            OnPropertyChanged(nameof(IsKartActive));
            OnPropertyChanged(nameof(IsScoreActive));
        }

        [RelayCommand]
        private void ShowSettings(MethodSettingsType type)
        {
            if (SelectedHeat == null)
                return;

            CurrentSettingsType = type;

            var config = _methodConfigurations[type];
            CurrentActiveSettings = config.GetExistingVm();
            CurrentActiveSettingsTitle = config.AvailableMethods
                .FirstOrDefault(x => x.Id == config.GetSelectedMethodId())?.Title;
        }

        private record MethodConfiguration(
            Func<string?> GetSelectedMethodId,
            Func<MethodParameters?> GetExistingVm,
            ObservableCollection<MethodOption> AvailableMethods);
    }
}
