using Hekki.Application.DTOs.Pilot;
using Hekki.UI.ViewModels;
using Hekki.UI.Views;
using Hekki.UI.Views.Race;

namespace Hekki.UI.Services
{
    public class DialogService : IDialogService
    {
        public PilotDto? ShowPilotEditor(PilotEditorViewModel vm)
        {
            var window = new PilotEditorWindow { DataContext = vm };

            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(vm.DialogResult) &&
                    vm.DialogResult.HasValue)
                {
                    window.DialogResult = vm.DialogResult.Value;
                }
            };

            return window.ShowDialog() == true ? vm.Result : null;
        }

        public bool? ShowRaceSettings(RaceSettingsViewModel vm)
        {
            var window = new RaceSettingsWindow { DataContext = vm };

            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(vm.DialogResult) &&
                    vm.DialogResult.HasValue)
                {
                    window.DialogResult = vm.DialogResult.Value;
                }
            };

            return window.ShowDialog();
        }

        public bool? ShowMainSettings(MainSettingsViewModel vm)
        {
            var window = new MainSettingsWindow { DataContext = vm };

            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(vm.DialogResult) &&
                    vm.DialogResult.HasValue)
                {
                    window.DialogResult = vm.DialogResult.Value;
                }
            };

            return window.ShowDialog();
        }
    }
}
