using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Regulation;
using Hekki.UI.Enums;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Hekki.UI.ViewModels
{
    public partial class CreateRegulationViewModel : ObservableValidator
    {
        private readonly INavigationService _navigationService;
        private readonly IMethodCatalogService _methodCatalog;
        private readonly IRegulationRepository _regulationRepository;

        private readonly Dictionary<MethodSettingsType, MethodConfiguration> _methodConfigurations;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [Required(ErrorMessage = "Regulation name is required")]
        [MinLength(3, ErrorMessage = "Regulation name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Regulation name cannot exceed 100 characters")]
        [NotifyDataErrorInfo]
        private string _regulationName = string.Empty;
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

        public ObservableCollection<HeatConfigurationViewModel> Heats { get; } = [];
        public ObservableCollection<MethodOption> AvailableShuffleMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableGroupMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableKartMethods { get; } = [];
        public ObservableCollection<MethodOption> AvailableScoreMethods { get; } = [];

        public bool IsShuffleActive => CurrentSettingsType == MethodSettingsType.Shuffle;
        public bool IsGroupActive => CurrentSettingsType == MethodSettingsType.Group;
        public bool IsKartActive => CurrentSettingsType == MethodSettingsType.Kart;
        public bool IsScoreActive => CurrentSettingsType == MethodSettingsType.Score;

        public CreateRegulationViewModel(
            INavigationService navigationService,
            IMethodCatalogService methodCatalog,
            IRegulationRepository regulationRepository)
        {
            _navigationService = navigationService;
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
                    () => SelectedHeat?.ShuffleParameters,
                    AvailableShuffleMethods
                ),
                [MethodSettingsType.Group] = new(
                    () => SelectedGroupMethodId,
                    () => SelectedHeat?.GroupParameters,
                    AvailableGroupMethods
                ),
                [MethodSettingsType.Kart] = new(
                    () => SelectedKartMethodId,
                    () => SelectedHeat?.KartParameters,
                    AvailableKartMethods
                ),
                [MethodSettingsType.Score] = new(
                    () => SelectedScoreMethodId,
                    () => SelectedHeat?.ScoreParameters,
                    AvailableScoreMethods
                )
            };
        }

        partial void OnSelectedShuffleMethodIdChanged(string? value)
        {
            if (SelectedHeat == null) return;

            SelectedHeat.ShuffleMethodId = value ?? string.Empty;
            SelectedHeat.ShuffleParameters = CreateParametersVm(value);
            HasShuffleSettings = SelectedHeat.ShuffleParameters != null;
        }

        partial void OnSelectedGroupMethodIdChanged(string? value)
        {
            if (SelectedHeat == null) return;

            SelectedHeat.GroupMethodId = value ?? string.Empty;
            SelectedHeat.GroupParameters = CreateParametersVm(value);
            HasGroupSettings = SelectedHeat.GroupParameters != null;
        }

        partial void OnSelectedKartMethodIdChanged(string? value)
        {
            if (SelectedHeat == null) return;

            SelectedHeat.KartMethodId = value ?? string.Empty;
            SelectedHeat.KartParameters = CreateParametersVm(value);
            HasKartSettings = SelectedHeat.KartParameters != null;
        }

        partial void OnSelectedScoreMethodIdChanged(string? value)
        {
            if (SelectedHeat == null) return;

            SelectedHeat.ScoreMethodId = value ?? string.Empty;
            SelectedHeat.ScoreParameters = CreateParametersVm(value);
            HasScoreSettings = SelectedHeat.ScoreParameters != null;
        }

        partial void OnSelectedHeatChanged(HeatConfigurationViewModel? value)
        {
            if (value == null)
            {
                HasShuffleSettings = false;
                HasGroupSettings = false;
                HasKartSettings = false;
                HasScoreSettings = false;
                CurrentActiveSettings = null;
                CurrentSettingsType = null;
                return;
            }

            SelectedShuffleMethodId = value.ShuffleMethodId;
            SelectedGroupMethodId = value.GroupMethodId;
            SelectedKartMethodId = value.KartMethodId;
            SelectedScoreMethodId = value.ScoreMethodId;

            HasShuffleSettings = value.ShuffleParameters != null;
            HasGroupSettings = value.GroupParameters != null;
            HasKartSettings = value.KartParameters != null;
            HasScoreSettings = value.ScoreParameters != null;

            CurrentActiveSettings = null;
            CurrentSettingsType = null;
        }

        partial void OnCurrentSettingsTypeChanged(MethodSettingsType? value)
        {
            OnPropertyChanged(nameof(IsShuffleActive));
            OnPropertyChanged(nameof(IsGroupActive));
            OnPropertyChanged(nameof(IsKartActive));
            OnPropertyChanged(nameof(IsScoreActive));
        }

        private MethodParameters? CreateParametersVm(string? methodId) => methodId switch
        {
            "replacement_group_assignment" => new ReplacementParameters(),
            _ => null
        };

        [RelayCommand]
        private void AddHeat()
        {
            int nextNumber = Heats.Count + 1;
            var newHeat = new HeatConfigurationViewModel
            {
                Name = $"Heat {nextNumber}",
                HeatNumber = nextNumber,
                UsePoints = true,
                UseTime = false
            };
            Heats.Add(newHeat);
            SelectedHeat = Heats.Last();
        }

        [RelayCommand]
        private void DeleteHeat()
        {
            if (SelectedHeat == null) return;

            var heatToRemove = SelectedHeat;
            var index = Heats.IndexOf(heatToRemove);

            Heats.Remove(heatToRemove);

            for (int i = 0; i < Heats.Count; i++)
            {
                Heats[i].Name = $"Heat {i + 1}";
                Heats[i].HeatNumber = i + 1;
            }

            if (Heats.Any())
            {
                SelectedHeat = index < Heats.Count ? Heats[index] : Heats.Last();
            }
            else
            {
                SelectedHeat = null;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _navigationService.NavigateToSelection();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                WeakReferenceMessenger.Default.Send(new AppErrorMessage($"Validation failed:\n{errors}"));
                return;
            }

            if (!Heats.Any())
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage("At least one heat is required"));
                return;
            }

            // Валидация: хотя бы один тип счёта должен быть выбран для каждого хита
            var invalidHeats = Heats.Where(h => !h.IsValid()).ToList();
            if (invalidHeats.Any())
            {
                var heatNames = string.Join(", ", invalidHeats.Select(h => h.Name));
                WeakReferenceMessenger.Default.Send(new AppErrorMessage($"Please select at least one scoring type (Points or Time) for: {heatNames}"));
                return;
            }

            try
            {
                var heatConfigs = Heats.Select(h => new HeatConfig
                {
                    Name = h.Name,
                    HeatNumber = h.HeatNumber,
                    GroupCount = h.NumberOfGroups,
                    ParticipantsPerGroup = h.GroupCapacity,
                    ScoringMode = h.ScoringMode,
                    Scoring = new ScoringConfig
                    {
                        Method = _methodCatalog.CreateScoreMethod(h.ScoreMethodId),
                        UsePenalties = h.UsePenalty,
                    },
                    Assignment = new AssignmentConfig
                    {
                        KartMethod = _methodCatalog.CreateKartMethod(h.KartMethodId),
                        GroupMethod = _methodCatalog.CreateGroupMethod(h.GroupMethodId),
                        Shuffle = _methodCatalog.CreateShuffleMethod(h.ShuffleMethodId)
                    }
                }).ToList();

                var dto = new RegulationEditDto
                {
                    Name = RegulationName,
                    Config = new RegulationConfig
                    {
                        HeatConfigs = heatConfigs
                    }
                };


                var regulationId = await _regulationRepository.AddAsync(dto);

                WeakReferenceMessenger.Default.Send(new AppSuccessMessage("Regulation saved successfully!"));

                _navigationService.NavigateToRace(regulationId);
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage($"Failed to save: {ex.Message}"));
            }
        }

        [RelayCommand]
        private void ShowSettings(MethodSettingsType type)
        {
            if (SelectedHeat == null) return;

            CurrentSettingsType = type;

            var config = _methodConfigurations[type];
            CurrentActiveSettings = config.GetExistingVm();
            CurrentActiveSettingsTitle = config.AvailableMethods
                .FirstOrDefault(x => x.Id == config.GetSelectedMethodId())?.Title;
        }

        private bool CanSave() => !HasErrors && !string.IsNullOrWhiteSpace(RegulationName);

        private record MethodConfiguration(
                Func<string?> GetSelectedMethodId,
                Func<MethodParameters?> GetExistingVm,
                ObservableCollection<MethodOption> AvailableMethods);
    }
}