using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public static class PilotUiMapper
    {
        public static PilotViewModel MapToPilotViewModel(PilotDto pilot)
        {
            return new PilotViewModel
            {
                PilotId = pilot.Id,
                FirstName = pilot.FirstName,
                LastName = pilot.LastName,
            };
        }

        public static RaceParticipantViewModel MapToParticipantViewModel(RaceParticipantDto raceParticipant)
        {
            return new RaceParticipantViewModel
            {
                Id = raceParticipant.ParticipantId,
                PilotId = raceParticipant.PilotId,
                FirstName = raceParticipant.FirstName,
                LastName = raceParticipant.LastName,
                PilotPhotoPath = raceParticipant.PhotoPath,
                Team = raceParticipant.Team,
                League = raceParticipant.League,
                IsActive = raceParticipant.IsActive
            };
        }
    }
}
