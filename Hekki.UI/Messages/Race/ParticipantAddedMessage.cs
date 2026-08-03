using Hekki.Application.DTOs.Race;

namespace Hekki.UI.Messages.Race
{
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);

}
