using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstrations;
using Hekki.Application.Methods;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class CreateRaceViewModel : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRegulationRepository _regulationRepository;

        private string _regulationName = string.Empty;
        private readonly IParticipantShuffleCatalog _shuffleCatalog;
        private readonly IGroupAssignmentCatalog _groupCatalog;
        private readonly IKartNummerAssignmentCatalog _kartCatalog;
        private readonly IScoreAssignmentCatalog _scoreCatalog;
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
            NavigationService navigationService,
            IViewModelFactory viewModelFactory,
            IParticipantShuffleCatalog shuffleCatalog,
            IGroupAssignmentCatalog groupCatalog,
            IKartNummerAssignmentCatalog kartCatalog,
            IScoreAssignmentCatalog scoreCatalog,
            IRegulationRepository regulationRepository)
        {
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;
            _shuffleCatalog = shuffleCatalog;
            _groupCatalog = groupCatalog;
            _kartCatalog = kartCatalog;
            _scoreCatalog = scoreCatalog;
            _regulationRepository = regulationRepository;

            Fill(AvailableShuffleMethods, _shuffleCatalog.GetAll());
            Fill(AvailableGroupMethods, _groupCatalog.GetAll());
            Fill(AvailableKartMethods, _kartCatalog.GetAll());
            Fill(AvailableScoreMethods, _scoreCatalog.GetAll());

            _selectedShuffleMethodId = AvailableShuffleMethods.FirstOrDefault()?.Id;
            _selectedGroupMethodId = AvailableGroupMethods.FirstOrDefault()?.Id;
            _selectedKartMethodId = AvailableKartMethods.FirstOrDefault()?.Id;
            _selectedScoreMethodId = AvailableScoreMethods.FirstOrDefault()?.Id;
        }

        private MethodParametersViewModel? CreateParametersVm(string? methodId) => methodId switch
        {
            "replacement_group_assignment" => new ReplacementParametersViewModel(),
            _ => null
        };

        partial void OnSelectedGroupMethodIdChanged(string? value)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat != null)
            {
                SelectedHeat.Grouping.MethodId = value ?? string.Empty;
                SelectedHeat.ActiveGroupingSettings = CreateParametersVm(value);
                CurrentActiveSettings = SelectedHeat.ActiveGroupingSettings;
                CurrentActiveSettingsTitle = AvailableGroupMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
            }
        }

        partial void OnSelectedShuffleMethodIdChanged(string? value)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat != null)
            {
                SelectedHeat.Shuffle.MethodId = value ?? string.Empty;
                SelectedHeat.ActiveShuffleSettings = CreateParametersVm(value);
                CurrentActiveSettings = SelectedHeat.ActiveShuffleSettings;
                CurrentActiveSettingsTitle = AvailableShuffleMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
            }
        }

        partial void OnSelectedKartMethodIdChanged(string? value)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat != null)
            {
                SelectedHeat.KartAssignment.MethodId = value ?? string.Empty;
                SelectedHeat.ActiveKartSettings = CreateParametersVm(value);
                CurrentActiveSettings = SelectedHeat.ActiveKartSettings;
                CurrentActiveSettingsTitle = AvailableKartMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
            }
        }

        partial void OnSelectedScoreMethodIdChanged(string? value)
        {
            IsAdditionalSettingsPanelActivated = true;
            if (SelectedHeat != null)
            {
                SelectedHeat.Scoring.MethodId = value ?? string.Empty;
                SelectedHeat.ActiveScoringSettings = CreateParametersVm(value);
                CurrentActiveSettings = SelectedHeat.ActiveScoringSettings;
                CurrentActiveSettingsTitle = AvailableScoreMethods.FirstOrDefault(n => n.Id == value)?.Title ?? string.Empty;
            }
        }

        partial void OnSelectedHeatChanged(HeatConfiguration? value)
        {
            CurrentActiveSettings = null;
        }

        private static void Fill<TMethod>(
            ObservableCollection<MethodOptionViewModel> target,
            IReadOnlyList<TMethod> methods)
            where TMethod : class
        {
            target.Clear();

            foreach (dynamic m in methods)
                target.Add(new MethodOptionViewModel(m.Id, m.Title, m.Description));
        }

        [RelayCommand]
        private void AddHeat()
        {
            int nextNumber = Heats.Count + 1;
            Heats.Add(new HeatConfiguration { Name = $"Heat {nextNumber}" });
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                await _regulationRepository.AddAsync(RegulationUiMapper.ToDomain(this, 0, 1, DateTime.Now));
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