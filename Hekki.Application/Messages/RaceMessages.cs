using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Messages
{
    public record RaceCreatedMessage(int RaceId);
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);
    public record ParticipantRemovedMessage(int RaceId, Guid ParticipantId);
    public record HeatGeneratedMessage(int RaceId, int HeatId);
    public record GroupsAssignedMessage(int RaceId, int HeatId, List<GroupAssignmentResultDto> Result);
}