using Hekki.Domain.Models;

namespace Hekki.Application.Methods
{
    public class DefaultScoreAssignment : IScoreAssignmentMethod
    {
        public string Id => "default";
        public string Title => "Default Score Assignment";
        public string Description => "Assigns scores using the default method.";

        public List<RaceParticipant> AssignScores(List<RaceParticipant> participants, List<int> scores)
        {
            return participants;
        }
    }
}
