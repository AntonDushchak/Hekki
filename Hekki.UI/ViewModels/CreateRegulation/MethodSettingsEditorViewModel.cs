using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class MethodSettingsEditorViewModel : ObservableObject
    {
        private readonly Dictionary<MethodSettingsType, MethodConfiguration> _methodConfigurations;
        private bool _isSynchronizingHeat;

        [ObservableProperty] private HeatConfigurationViewModel? _selectedHeat;

        [ObservableProperty] private string? _selectedShuffleMethodId;
        [ObservableProperty] private string? _selectedGroupMethodId;
        [ObservableProperty] private string? _selectedKartMethodId;
        [ObservableProperty] private string? _selectedScoreMethodId;

        [ObservableProperty] private MethodParameters? _currentActiveSettings;
        [ObservableProperty] private string? _currentActiveSettingsTitle;
        [ObservableProperty] private MethodSettingsType? _currentSettingsType;

        [ObservableProperty] private bool _hasShuffleSettings;
        [ObservableProperty] private bool _hasGroupSettings;
        [ObservableProperty] private bool _hasKartSettings;
        [ObservableProperty] private bool _hasScoreSettings;

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
                    () => SelectedShuffleMethodId,
                    () => SelectedHeat?.ShuffleParameters,
                    AvailableShuffleMethods),
                [MethodSettingsType.Group] = new(
                    () => SelectedGroupMethodId,
                    () => SelectedHeat?.GroupParameters,
                    AvailableGroupMethods),
                [MethodSettingsType.Kart] = new(
                    () => SelectedKartMethodId,
                    () => SelectedHeat?.KartParameters,
                    AvailableKartMethods),
                [MethodSettingsType.Score] = new(
                    () => SelectedScoreMethodId,
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

            _isSynchronizingHeat = true;
            try
            {
                SelectedShuffleMethodId = value.ShuffleMethodId;
                SelectedGroupMethodId = value.GroupMethodId;
                SelectedKartMethodId = value.KartMethodId;
                SelectedScoreMethodId = value.ScoreMethodId;

                HasShuffleSettings = value.ShuffleParameters != null;
                HasGroupSettings = value.GroupParameters != null;
                HasKartSettings = value.KartParameters != null;
                HasScoreSettings = value.ScoreParameters != null;

                CurrentActiveSettings = null;
                CurrentActiveSettingsTitle = null;
                CurrentSettingsType = null;
            }
            finally
            {
                _isSynchronizingHeat = false;
            }
        }

        partial void OnSelectedShuffleMethodIdChanged(string? value)
        {
            UpdateMethodSelection(MethodSettingsType.Shuffle, value);
        }

        partial void OnSelectedGroupMethodIdChanged(string? value)
        {
            UpdateMethodSelection(MethodSettingsType.Group, value);
        }

        partial void OnSelectedKartMethodIdChanged(string? value)
        {
            UpdateMethodSelection(MethodSettingsType.Kart, value);
        }

        partial void OnSelectedScoreMethodIdChanged(string? value)
        {
            UpdateMethodSelection(MethodSettingsType.Score, value);
        }

        private void UpdateMethodSelection(MethodSettingsType type, string? methodId)
        {
            if (_isSynchronizingHeat || SelectedHeat == null)
                return;

            var parameters = CreateParametersVm(methodId);

            switch (type)
            {
                case MethodSettingsType.Shuffle:
                    SelectedHeat.ShuffleMethodId = methodId ?? string.Empty;
                    SelectedHeat.ShuffleParameters = parameters;
                    HasShuffleSettings = parameters != null;
                    break;

                case MethodSettingsType.Group:
                    SelectedHeat.GroupMethodId = methodId ?? string.Empty;
                    SelectedHeat.GroupParameters = parameters;
                    HasGroupSettings = parameters != null;
                    break;

                case MethodSettingsType.Kart:
                    SelectedHeat.KartMethodId = methodId ?? string.Empty;
                    SelectedHeat.KartParameters = parameters;
                    HasKartSettings = parameters != null;
                    break;

                case MethodSettingsType.Score:
                    SelectedHeat.ScoreMethodId = methodId ?? string.Empty;
                    SelectedHeat.ScoreParameters = parameters;
                    HasScoreSettings = parameters != null;
                    break;
            }
        }

        private MethodParameters? CreateParametersVm(string? methodId) => methodId switch
        {
            "replacement_group_assignment" => new ReplacementParameters(),
            _ => null
        };

        private void ResetMethodSettingsState()
        {
            _isSynchronizingHeat = true;
            try
            {
                SelectedShuffleMethodId = null;
                SelectedGroupMethodId = null;
                SelectedKartMethodId = null;
                SelectedScoreMethodId = null;
            }
            finally
            {
                _isSynchronizingHeat = false;
            }

            HasShuffleSettings = false;
            HasGroupSettings = false;
            HasKartSettings = false;
            HasScoreSettings = false;
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
