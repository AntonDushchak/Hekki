using Hekki.Application.DTOs;

namespace Hekki.Application.Methods
{
    public class DefaultScoreAssignment : IScoreAssignmentMethod
    {
        public string Id => "default";
        public string Title => "Default Score Assignment";
        public string Description => "Assigns scores using the default method.";

        public List<PilotDto> AssignScores(List<PilotDto> participants, List<int> scores)
        {
            return participants;
        }
    }
}
