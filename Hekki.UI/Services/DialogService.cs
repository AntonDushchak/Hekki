using Hekki.UI.ViewModels;
using Hekki.UI.Views.Race;

namespace Hekki.UI.Services
{
    public class DialogService : IDialogService
    {
        public bool? ShowRaceSettings(RaceSettingsViewModel vm)
        {
            var window = new RaceSettingsWindow { DataContext = vm };
            return window.ShowDialog();
        }
    }
}
