using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Messages;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Hekki.UI.ViewModels
{
    public partial class CreateRegulationViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMethodCatalogService _methodCatalog;
        private readonly IRegulationRepository _regulationRepository;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [Required(ErrorMessage = "Regulation name is required")]
        [MinLength(3, ErrorMessage = "Regulation name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Regulation name cannot exceed 100 characters")]
        [NotifyDataErrorInfo]
        private string _regulationName = string.Empty;
        [ObservableProperty] private HeatConfigurationViewModel? _selectedHeat;

        public ObservableCollection<HeatConfigurationViewModel> Heats { get; } = [];
        public MethodSettingsEditorViewModel MethodSettingsEditor { get; }

        public CreateRegulationViewModel(
            INavigationService navigationService,
            IMethodCatalogService methodCatalog,
            IRegulationRepository regulationRepository)
        {
            _navigationService = navigationService;
            _methodCatalog = methodCatalog;
            _regulationRepository = regulationRepository;

            MethodSettingsEditor = new MethodSettingsEditorViewModel(methodCatalog);
        }

        partial void OnSelectedHeatChanged(HeatConfigurationViewModel? value)
        {
            MethodSettingsEditor.SelectedHeat = value;
        }


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
        private Task Save() => ExecuteSafeAsync(async () =>
        {
            ValidateAllProperties();
            ValidateForm();


            var dto = BuildRegulationDto();

            var regulationId = await _regulationRepository.AddAsync(dto);

            WeakReferenceMessenger.Default.Send(new AppSuccessMessage("Regulation saved successfully!"));

            _navigationService.NavigateToRace(regulationId);

        });

        private void ValidateForm()
        {
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

            var invalidHeats = Heats.Where(h => !h.IsValid()).ToList();
            if (invalidHeats.Count > 0)
            {
                var heatNames = string.Join(", ", invalidHeats.Select(h => h.Name));
                WeakReferenceMessenger.Default.Send(new AppErrorMessage($"Please select at least one scoring type (Points or Time) for: {heatNames}"));
                return;
            }
        }

        private RegulationEditDto BuildRegulationDto()
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

            return new RegulationEditDto
            {
                Name = RegulationName,
                Config = new RegulationConfig
                {
                    HeatConfigs = heatConfigs
                }
            };
        }

        private bool CanSave() => !HasErrors && !string.IsNullOrWhiteSpace(RegulationName);
    }
}