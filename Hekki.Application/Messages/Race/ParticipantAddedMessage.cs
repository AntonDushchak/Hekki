using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Messages.Race
{
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);

}
