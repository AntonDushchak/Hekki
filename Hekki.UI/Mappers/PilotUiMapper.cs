using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{
    public static class PilotUiMapper
    {
        public static PilotViewModel MapToViewModel(PilotDto pilot)
        {
            return new PilotViewModel
            {
                PilotId = pilot.Id,
                FirstName = pilot.FirstName,
                LastName = pilot.LastName,
            };
        }

        public static void ApplyToParticipantViewModel(RaceParticipantDto raceParticipant, RaceParticipantViewModel participant)
        {
            participant.FirstName = raceParticipant.FirstName;
            participant.LastName = raceParticipant.LastName;
            participant.PilotPhotoPath = raceParticipant.PhotoPath;
            participant.PilotProfileUrl = raceParticipant.ProfileUrl;
            participant.Team = raceParticipant.Team;
            participant.League = raceParticipant.League;
            participant.IsActive = raceParticipant.IsActive;
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
                PilotProfileUrl = raceParticipant.ProfileUrl,
                Team = raceParticipant.Team,
                League = raceParticipant.League,
                IsActive = raceParticipant.IsActive
            };
        }
    }
}
