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

        private string _regulationName = string.Empty;
        [ObservableProperty] private string? _selectedShuffleMethodId;
        [ObservableProperty] private string? _selectedGroupMethodId;
        [ObservableProperty] private string? _selectedKartMethodId;
        [ObservableProperty] private string? _selectedScoreMethodId;
        [ObservableProperty] private HeatConfiguration? _selectedHeat;
        [ObservableProperty] private MethodParametersViewModel? _currentActiveSettings;
        [ObservableProperty] private string? _currentActiveSettingsTitle;
        [ObservableProperty] private bool _isAdditionalSettingsPanelActivated = false;

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

            SetDefaultSelectedMethods();
        }

        private void SetDefaultSelectedMethods()
        {
            SelectedShuffleMethodId = AvailableShuffleMethods.FirstOrDefault()?.Id;
            SelectedGroupMethodId = AvailableGroupMethods.FirstOrDefault()?.Id;
            SelectedKartMethodId = AvailableKartMethods.FirstOrDefault()?.Id;
            SelectedScoreMethodId = AvailableScoreMethods.FirstOrDefault()?.Id;
        }

        

        partial void OnSelectedGroupMethodIdChanged(string? value)
        {
            UpdateSelectedMethod(
                value,
                v => SelectedHeat!.Grouping.MethodId = v,
                vm => SelectedHeat!.ActiveGroupingSettings = vm,
                AvailableGroupMethods,
                SelectedHeat?.ActiveGroupingSettings);
        }

        partial void OnSelectedShuffleMethodIdChanged(string? value)
        {
            UpdateSelectedMethod(
                value,
                v => SelectedHeat!.Shuffle.MethodId = v,
                vm => SelectedHeat!.ActiveShuffleSettings = vm,
                AvailableShuffleMethods, 
                SelectedHeat?.ActiveShuffleSettings);
        }

        partial void OnSelectedKartMethodIdChanged(string? value)
        {
            UpdateSelectedMethod(
                value,
                v => SelectedHeat!.KartAssignment.MethodId = v,
                vm => SelectedHeat!.ActiveKartSettings = vm,
                AvailableKartMethods,
                SelectedHeat?.ActiveKartSettings);
        }

        partial void OnSelectedScoreMethodIdChanged(string? value)
        {
            UpdateSelectedMethod(
                value,
                v => SelectedHeat!.Scoring.MethodId = v,
                vm => SelectedHeat!.ActiveScoringSettings = vm,
                AvailableScoreMethods,
                SelectedHeat?.ActiveScoringSettings);
        }
        private void SyncHeatWithDefaults(HeatConfiguration heat)
        {
            if (heat == null) return;

            UpdateSelectedMethod(SelectedShuffleMethodId,
                v => heat.Shuffle.MethodId = v,
                vm => heat.ActiveShuffleSettings = vm,
                AvailableShuffleMethods, heat.ActiveShuffleSettings);

            UpdateSelectedMethod(SelectedGroupMethodId,
                v => heat.Grouping.MethodId = v,
                vm => heat.ActiveGroupingSettings = vm,
                AvailableGroupMethods, heat.ActiveGroupingSettings);

            UpdateSelectedMethod(SelectedKartMethodId,
                v => heat.KartAssignment.MethodId = v,
                vm => heat.ActiveKartSettings = vm,
                AvailableKartMethods, heat.ActiveKartSettings);

            UpdateSelectedMethod(SelectedScoreMethodId,
                v => heat.Scoring.MethodId = v,
                vm => heat.ActiveScoringSettings = vm,
                AvailableScoreMethods, heat.ActiveScoringSettings);
        }
        private void UpdateSelectedMethod(
            string? value,
            Action<string> setMethodId,
            Action<MethodParametersViewModel?> setActiveSettings,
            ObservableCollection<MethodOptionViewModel> availableMethods,
            MethodParametersViewModel? existingVm)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat == null) return;


            setMethodId(value ?? string.Empty);

            var vm = existingVm;
            if (vm == null || GetIdByVm(vm) != value)
            {
                vm = CreateParametersVm(value);
                setActiveSettings(vm);
            }

            CurrentActiveSettings = vm;
            CurrentActiveSettingsTitle = availableMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
        }

        partial void OnSelectedHeatChanged(HeatConfiguration? value)
        {
            CurrentActiveSettings = null;
            SetDefaultSelectedMethods();
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

        [RelayCommand]
        private void AddHeat()
        {
            int nextNumber = Heats.Count + 1;
            Heats.Add(new HeatConfiguration { Name = $"Heat {nextNumber}" });
            SelectedHeat = Heats.Last();
            SyncHeatWithDefaults(Heats.Last());
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
        private void SelectSettings(string type)
        {
            IsAdditionalSettingsPanelActivated = true;

            switch (type)
            {
                case "Shuffle":
                    CurrentActiveSettings = SelectedHeat?.ActiveShuffleSettings;
                    CurrentActiveSettingsTitle = AvailableShuffleMethods.FirstOrDefault(x => x.Id == SelectedShuffleMethodId)?.Title;
                    break;
                case "Group":
                    CurrentActiveSettings = SelectedHeat?.ActiveGroupingSettings;
                    CurrentActiveSettingsTitle = AvailableGroupMethods.FirstOrDefault(x => x.Id == SelectedGroupMethodId)?.Title;
                    break;
                case "Kart":
                    CurrentActiveSettings = SelectedHeat?.ActiveKartSettings;
                    CurrentActiveSettingsTitle = AvailableKartMethods.FirstOrDefault(x => x.Id == SelectedKartMethodId)?.Title;
                    break;
                case "Score":
                    CurrentActiveSettings = SelectedHeat?.ActiveScoringSettings;
                    CurrentActiveSettingsTitle = AvailableScoreMethods.FirstOrDefault(x => x.Id == SelectedScoreMethodId)?.Title;
                    break;
            }

        }
    }
}