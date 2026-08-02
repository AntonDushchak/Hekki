using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Methods
{
    public class DefaultScoreAssignment : IScoreAssignmentMethod
    {
        public string Id => "default";
        public string Title => "Default Score Assignment";
        public string Description => "Assigns scores using the default method.";

        public List<RaceParticipantDto> AssignScores(List<RaceParticipantDto> participants, List<int> scores)
        {
            return participants;
        }
    }
}
