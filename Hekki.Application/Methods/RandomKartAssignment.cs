using Hekki.Application.Models;

namespace Hekki.Application.Methods
{
    public class RandomKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_kart_assignment";
        public string Title => "Random Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants.";

        public void AssignKartNummer(
            IReadOnlyList<ParticipantAssignment> assignments,
            IReadOnlyDictionary<int, IReadOnlyList<int>> previousKartNumbers,
            IReadOnlyList<int> availableKarts)
        {
            for (var i = 0; i < assignments.Count; i++)
            {
                assignments[i].KartNumber = availableKarts[i];
            }
        }
    }

    public class RandomNoRepeatKartAssignment : IKartNummerAssignmentMethod
    {
        public string Id => "random_no_repeat_kart_assignment";
        public string Title => "Random No Repeat Kart Assignment";
        public string Description => "Randomly assigns kart numbers to participants without repetition.";

        public void AssignKartNummer(
            IReadOnlyList<ParticipantAssignment> assignments,
            IReadOnlyDictionary<int, IReadOnlyList<int>> previousKartNumbers,
            IReadOnlyList<int> availableKarts)
        {
            throw new NotImplementedException();
        }
    }
}
