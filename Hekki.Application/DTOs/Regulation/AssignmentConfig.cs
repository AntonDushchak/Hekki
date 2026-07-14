using Hekki.Application.Methods;

namespace Hekki.Application.DTOs.Regulation
{
    public class AssignmentConfig
    {
        public IKartNummerAssignmentMethod KartMethod { get; set; }
        public IGroupAssignmentMethod GroupMethod { get; set; }
        public IParticipantShuffleMethod Shuffle { get; set; }
    }
}
