using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Messages
{
    public record ParticipantAddedMessage(int RaceId, RaceParticipantDto RaceParticipant);
    public record ParticipantUpdatedMessage(int RaceId, RaceParticipantDto RaceParticipant);
    public record ParticipantRemovedMessage(int RaceId, Guid ParticipantId);
    public record ParticipantsReorderedMessage(int RaceId, IReadOnlyList<Guid> ParticipantIds);
    public record HeatResultChangedMessage(int RaceId, int HeatId, HeatResultDto Result);
    public record HeatAssignmentClearedMessage(int RaceId, int HeatId);
    public record GroupsAssignedMessage(int RaceId, int HeatId, List<GroupAssignmentResultDto> Result);
}