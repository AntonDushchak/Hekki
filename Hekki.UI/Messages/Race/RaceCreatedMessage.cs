using Hekki.Application.DTOs.Race;

namespace Hekki.UI.Messages.Race
{
    public record RaceCreatedMessage(int RaceId);
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);
    public record ParticipantRemovedMessage(int RaceId, int ParticipantId);
    public record HeatGeneratedMessage(int RaceId, int HeatId);
    public record GroupsAssignedMessage(int RaceId, int HeatId);

}
