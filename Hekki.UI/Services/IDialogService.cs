using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public interface IDialogService
    {
        bool? ShowRaceSettings(RaceSettingsViewModel vm);
    }
}