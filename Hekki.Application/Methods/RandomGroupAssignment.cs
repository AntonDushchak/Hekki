using Hekki.Domain.Models;

namespace Hekki.Application.Methods
{
    public class RandomGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "random_group_assignment";
        public string Title => "Random Group Assignment";
        public string Description => "Randomly assigns participants to groups.";


        public List<List<RaceParticipant>> AssignGroups(List<RaceParticipant> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }

    public class CardGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "card_group_assignment";
        public string Title => "Card Group Assignment";
        public string Description => "Assigns participants to groups based on cards.";

        public List<List<RaceParticipant>> AssignGroups(List<RaceParticipant> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }

    public class ListGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "list_group_assignment";
        public string Title => "List Group Assignment";
        public string Description => "Assigns participants to groups based on a list.";

        public List<List<RaceParticipant>> AssignGroups(List<RaceParticipant> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }

    public class ReplacementGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "replacement_group_assignment";
        public string Title => "Replacement Group Assignment";
        public string Description => "Assigns participants to groups based on a replacement strategy.";

        public List<List<RaceParticipant>> AssignGroups(List<RaceParticipant> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }
}
