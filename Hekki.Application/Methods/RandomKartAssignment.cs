using Hekki.Domain.Models;

namespace Hekki.Application.Methods
{
    public class RandomKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_kart_assignment";
        public string Title => "Random Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants.";

        public List<RaceParticipant> AssignKartNummer(List<RaceParticipant> participants, List<int> kartNummers)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomNoRepeatKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_no_repeat_kart_assignment";
        public string Title => "Random No Repeat Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants without repetition.";

        public List<RaceParticipant> AssignKartNummer(List<RaceParticipant> participants, List<int> kartNummers)
        {
            throw new NotImplementedException();
        }
    }
}
