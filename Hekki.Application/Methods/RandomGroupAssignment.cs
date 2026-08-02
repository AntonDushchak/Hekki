using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Methods
{
    public class RandomGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "random_group_assignment";
        public string Title => "Random Group Assignment";
        public string Description => "Randomly assigns participants to groups.";


        public List<List<ParticipantAssignmentDto>> AssignGroups(List<RaceParticipantDto> participants, int groupSize, int groupCount)
        {
            var groups = new List<List<ParticipantAssignmentDto>>();
            int countGroups = (int)Math.Ceiling((double)participants.Count / groupSize);

            for (int i = 0; i < groupCount; i++)
                groups.Add(new List<ParticipantAssignmentDto>());

            for (int i = 0, j = 0; i < participants.Count; i++, j++)
            {
                if (j == groupCount)
                    j = 0;
                groups[j].Add(new ParticipantAssignmentDto() 
                { 
                    ParticipantId = participants[i].ParticipantId,
                    GridPosition = i,
                });
            }
            return groups;
        }
    }

    public class CardGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "card_group_assignment";
        public string Title => "Card Group Assignment";
        public string Description => "Assigns participants to groups based on cards.";

        public List<List<ParticipantAssignmentDto>> AssignGroups(List<RaceParticipantDto> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }

    public class ListGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "list_group_assignment";
        public string Title => "List Group Assignment";
        public string Description => "Assigns participants to groups based on a list.";

        public List<List<ParticipantAssignmentDto>> AssignGroups(List<RaceParticipantDto> participants, int groupSize, int groupCount)
        {
            var groups = new List<List<ParticipantAssignmentDto>>();
            int countGroups = (int)Math.Ceiling((double)participants.Count / groupSize);

            for (int i = 0; i < groupCount; i++)
                groups.Add(new List<ParticipantAssignmentDto>());

            int gridPosition = 1;
            for (int i = 0, j = 0; i < participants.Count; i++, j++)
            {
                if (j == groupCount)
                {  
                    gridPosition++; 
                    j = 0;
                }
                groups[j].Add(new ParticipantAssignmentDto()
                {
                    ParticipantId = participants[i].ParticipantId,
                    GridPosition = gridPosition,
                });
            }
            return groups;
        }
    }

    public class ReplacementGroupAssignment : IGroupAssignmentMethod
    {
        public string Id => "replacement_group_assignment";
        public string Title => "Replacement Group Assignment";
        public string Description => "Assigns participants to groups based on a replacement strategy.";
        private readonly int _toDown;
        private readonly int _toUp;

        public List<List<ParticipantAssignmentDto>> AssignGroups(List<RaceParticipantDto> participants, int groupSize, int groupCount)
        {
            throw new NotImplementedException();
        }
    }
}
