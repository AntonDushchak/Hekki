using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hekki.Application.DTOs.Regulation;
using Hekki.Application.Abstractions;
using Hekki.UI.Mappers;
using Hekki.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Hekki.UI.ViewModels
{
    public partial class CreateRegulationViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMethodCatalogService _methodCatalog;
        private readonly IRegulationService _regulationService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [LocalizedRequired("err_NameRequired")]
        [LocalizedMinLength(3, "err_NameTooShort")]
        [LocalizedMaxLength(100, "err_NameTooLong")]
        [NotifyDataErrorInfo]
        private string _regulationName = string.Empty;
        [ObservableProperty] private HeatConfigurationViewModel? _selectedHeat;

        public ObservableCollection<HeatConfigurationViewModel> Heats { get; } = [];
        public MethodSettingsEditorViewModel MethodSettingsEditor { get; }

        public CreateRegulationViewModel(
            INavigationService navigationService,
            IMethodCatalogService methodCatalog,
            IRegulationService regulationService)
        {
            _navigationService = navigationService;
            _methodCatalog = methodCatalog;
            _regulationService = regulationService;

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
                Name = Localizer.Get("m_DefaultHeatName", nextNumber),
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
                Heats[i].Name = Localizer.Get("m_DefaultHeatName", i + 1);
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
            if (!ValidateForm()) return;

            var dto = BuildRegulationDto();

            var regulationId = await _regulationService.AddRegulationAsync(dto);

            ShowSuccess(Localizer.Get("m_RegulationSaved"));

            await _navigationService.NavigateToRace(regulationId);
        });

        private bool ValidateForm()
        {
            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                ShowError($"{Localizer.Get("err_ValidationFailed")}\n{errors}");
                return false;
            }

            if (!Heats.Any())
            {
                ShowError(Localizer.Get("err_NoHeats"));
                return false;
            }

            var invalidHeats = Heats.Where(h => !h.IsValid()).ToList();
            if (invalidHeats.Count > 0)
            {
                var heatNames = string.Join(", ", invalidHeats.Select(h => h.Name));
                ShowError(Localizer.Get("err_NoScoringType", heatNames));
                return false;
            }

            return true;
        }

        private RegulationEditDto BuildRegulationDto()
        {
            var heatConfigs = Heats.Select(h => HeatConfigUiMapper.ToConfig(h, _methodCatalog)).ToList();

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