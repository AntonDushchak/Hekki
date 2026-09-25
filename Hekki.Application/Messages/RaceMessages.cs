using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Messages
{
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);
    public record ParticipantRemovedMessage(int RaceId, Guid ParticipantId);
    public record GroupsAssignedMessage(int RaceId, int HeatId, List<GroupAssignmentResultDto> Result);
}