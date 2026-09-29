using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Services
{
    public interface IDialogService
    {
        bool? ShowRaceSettings(RaceSettingsViewModel vm);
        PilotDto? ShowPilotEditor(PilotEditorViewModel vm);
        bool? ShowMainSettings(MainSettingsViewModel vm);
        bool Confirm(string title, string message, string confirmText);
        RaceSummaryDto? ShowLoadRace(LoadRaceViewModel vm);
    }
}