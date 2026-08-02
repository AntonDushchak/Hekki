using Hekki.Application.DTOs.Race;

namespace Hekki.Application.Methods
{
    public class RandomKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_kart_assignment";
        public string Title => "Random Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants.";

        public List<ParticipantAssignmentDto> AssignKartNummer(Dictionary<ParticipantAssignmentDto, List<int>> participantKartOptions, List<int> avaibleKarts)
        {
            int i = 0;
            var dtoList = new List<ParticipantAssignmentDto>();
            foreach (var item in participantKartOptions)
            {
                item.Value.Add(avaibleKarts[i]);
                i++;
                dtoList.Add(item.Key with { KartNumber = item.Value.Last() });
            }
            return dtoList;
        }
    }

    public class RandomNoRepeatKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_no_repeat_kart_assignment";
        public string Title => "Random No Repeat Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants without repetition.";

        public List<ParticipantAssignmentDto> AssignKartNummer(Dictionary<ParticipantAssignmentDto, List<int>> participantKartOptions, List<int> avaibleKarts)
        {
            throw new NotImplementedException();
        }
    }
}
