using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class CreateRaceViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IMethodCatalogService _methodCatalog;
        private readonly IRegulationRepository _regulationRepository;

        private readonly Dictionary<MethodSettingsType, MethodConfiguration> _methodConfigurations;

        private string _regulationName = string.Empty;
        [ObservableProperty] private string? _selectedShuffleMethodId;
        [ObservableProperty] private string? _selectedGroupMethodId;
        [ObservableProperty] private string? _selectedKartMethodId;
        [ObservableProperty] private string? _selectedScoreMethodId;
        [ObservableProperty] private HeatConfiguration? _selectedHeat;

        [ObservableProperty] private MethodParametersViewModel? _currentActiveSettings;
        [ObservableProperty] private string? _currentActiveSettingsTitle;
        [ObservableProperty] private bool _isAdditionalSettingsPanelActivated = false;
        [ObservableProperty] private MethodSettingsType? _currentSettingsType;

        public string RegulationName
        {
            get => _regulationName;
            set => SetProperty(ref _regulationName, value);
        }
        public ObservableCollection<HeatConfiguration> Heats { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableShuffleMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableGroupMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableKartMethods { get; } = [];
        public ObservableCollection<MethodOptionViewModel> AvailableScoreMethods { get; } = [];

        public bool HasShuffleSettings => GetHasSettings(MethodSettingsType.Shuffle);
        public bool HasGroupSettings => GetHasSettings(MethodSettingsType.Group);
        public bool HasKartSettings => GetHasSettings(MethodSettingsType.Kart);
        public bool HasScoreSettings => GetHasSettings(MethodSettingsType.Score);

        public bool IsShuffleActive => CurrentSettingsType == MethodSettingsType.Shuffle;
        public bool IsGroupActive => CurrentSettingsType == MethodSettingsType.Group;
        public bool IsKartActive => CurrentSettingsType == MethodSettingsType.Kart;
        public bool IsScoreActive => CurrentSettingsType == MethodSettingsType.Score;

        public CreateRaceViewModel(
            INavigationService navigationService,
            IViewModelFactory viewModelFactory,
            IMethodCatalogService methodCatalog,
            IRegulationRepository regulationRepository)
        {
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;
            _methodCatalog = methodCatalog;
            _regulationRepository = regulationRepository;

            foreach (var o in _methodCatalog.GetShuffleOptions()) AvailableShuffleMethods.Add(o);
            foreach (var o in _methodCatalog.GetGroupOptions()) AvailableGroupMethods.Add(o);
            foreach (var o in _methodCatalog.GetKartOptions()) AvailableKartMethods.Add(o);
            foreach (var o in _methodCatalog.GetScoreOptions()) AvailableScoreMethods.Add(o);

            _methodConfigurations = new()
            {
                [MethodSettingsType.Shuffle] = new(
                    () => SelectedShuffleMethodId,
                    v => SelectedHeat!.Shuffle.MethodId = v,
                    vm => SelectedHeat!.ActiveShuffleSettings = vm,
                    () => SelectedHeat?.ActiveShuffleSettings,
                    AvailableShuffleMethods
                ),
                [MethodSettingsType.Group] = new(
                    () => SelectedGroupMethodId,
                    v => SelectedHeat!.Grouping.MethodId = v,
                    vm => SelectedHeat!.ActiveGroupingSettings = vm,
                    () => SelectedHeat?.ActiveGroupingSettings,
                    AvailableGroupMethods
                ),
                [MethodSettingsType.Kart] = new(
                    () => SelectedKartMethodId,
                    v => SelectedHeat!.KartAssignment.MethodId = v,
                    vm => SelectedHeat!.ActiveKartSettings = vm,
                    () => SelectedHeat?.ActiveKartSettings,
                    AvailableKartMethods
                ),
                [MethodSettingsType.Score] = new(
                    () => SelectedScoreMethodId,
                    v => SelectedHeat!.Scoring.MethodId = v,
                    vm => SelectedHeat!.ActiveScoringSettings = vm,
                    () => SelectedHeat?.ActiveScoringSettings,
                    AvailableScoreMethods
                )
            };
        }

        partial void OnSelectedShuffleMethodIdChanged(string? value) =>
           UpdateMethod(MethodSettingsType.Shuffle, value);

        partial void OnSelectedGroupMethodIdChanged(string? value) =>
            UpdateMethod(MethodSettingsType.Group, value);

        partial void OnSelectedKartMethodIdChanged(string? value) =>
            UpdateMethod(MethodSettingsType.Kart, value);

        partial void OnSelectedScoreMethodIdChanged(string? value) =>
            UpdateMethod(MethodSettingsType.Score, value);

        private void UpdateMethod(MethodSettingsType type, string? value)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat == null) return;

            var config = _methodConfigurations[type];
            config.SetMethodId(value ?? string.Empty);

            var existingVm = config.GetExistingVm();
            var vm = existingVm;

            if (vm == null || GetIdByVm(vm) != value)
            {
                vm = CreateParametersVm(value);
                config.SetActiveSettings(vm);
            }

            CurrentActiveSettings = vm;
            CurrentActiveSettingsTitle = config.AvailableMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
        }

        partial void OnSelectedHeatChanged(HeatConfiguration? value)
        {
            CurrentActiveSettings = null;
            CurrentSettingsType = null;
        }

        private MethodParametersViewModel? CreateParametersVm(string? methodId) => methodId switch
        {
            "replacement_group_assignment" => new ReplacementParametersViewModel(),
            _ => null
        };

        private string GetIdByVm(MethodParametersViewModel? vm) => vm switch
        {
            ReplacementParametersViewModel => "replacement_group_assignment",
            _ => string.Empty
        };

        private bool GetHasSettings(MethodSettingsType type) =>
            _methodConfigurations[type].GetExistingVm() != null;


        [RelayCommand]
        private void AddHeat()
        {
            int nextNumber = Heats.Count + 1;
            Heats.Add(new HeatConfiguration { Name = $"Heat {nextNumber}" });
            SelectedHeat = Heats.Last();
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                await _regulationRepository.AddAsync(RegulationUiMapper.ToDomain(this, 0, 1, DateTime.UtcNow));
            }
            catch
            {
                //TODO: Handle error
            }

        }

        [RelayCommand]
        private void ShowSettings(MethodSettingsType type)
        {
            CurrentSettingsType = type;
            IsAdditionalSettingsPanelActivated = true;

            var config = _methodConfigurations[type];

            CurrentActiveSettings = config.GetExistingVm();
            CurrentActiveSettingsTitle = config.AvailableMethods
                .FirstOrDefault(x => x.Id == config.GetSelectedMethodId())?.Title;
        }

        private record MethodConfiguration(
                Func<string?> GetSelectedMethodId,
                Action<string> SetMethodId,
                Action<MethodParametersViewModel?> SetActiveSettings,
                Func<MethodParametersViewModel?> GetExistingVm,
                ObservableCollection<MethodOptionViewModel> AvailableMethods);
    }

    public enum MethodSettingsType
    {
        Shuffle,
        Group,
        Kart,
        Score
    }
}