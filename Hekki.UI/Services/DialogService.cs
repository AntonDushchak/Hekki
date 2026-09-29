using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;
using Hekki.UI.Views;
using Hekki.UI.Views.Race;
using Hekki.UI.Views.SelectRegulation;

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

        public RaceSummaryDto? ShowLoadRace(LoadRaceViewModel vm)
        {
            var window = new LoadRaceWindow { DataContext = vm };

            if (System.Windows.Application.Current?.MainWindow is { IsLoaded: true } owner)
                window.Owner = owner;

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

        public bool Confirm(string title, string message, string confirmText)
        {
            var window = new ConfirmWindow(title, message, confirmText);

            if (System.Windows.Application.Current?.MainWindow is { IsLoaded: true } owner)
                window.Owner = owner;

            return window.ShowDialog() == true;
        }
    }
}
