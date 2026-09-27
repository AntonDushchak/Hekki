namespace Hekki.Application.Exceptions
{
    public class ParticipantNotInHeatException(Guid participantId, int heatId) : AppException("err_ParticipantNotInHeat", participantId, heatId);
}
