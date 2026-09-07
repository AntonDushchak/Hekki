using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Messages.Race
{
    public record GroupsAssignedMessage(int RaceId, int HeatId, List<GroupAssignmentResultDto> Result);

}
