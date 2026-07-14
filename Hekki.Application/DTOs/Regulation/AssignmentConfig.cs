using Hekki.Application.Methods;

namespace Hekki.Application.DTOs.Regulation
{
    public class AssignmentConfig
    {
        public required IKartNummerAssignmentMethod KartMethod { get; init; }
        public required IGroupAssignmentMethod GroupMethod { get; init; }
        public required IParticipantShuffleMethod Shuffle { get; init; }
    }
}
