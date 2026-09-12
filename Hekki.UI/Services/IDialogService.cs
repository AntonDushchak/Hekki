using Hekki.Application.DTOs.Pilot;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public interface IDialogService
    {
        bool? ShowRaceSettings(RaceSettingsViewModel vm);
        PilotDto? ShowPilotEditor(PilotEditorViewModel vm);
    }
}