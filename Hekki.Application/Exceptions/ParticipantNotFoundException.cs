namespace Hekki.Application.Exceptions
{
    public class ParticipantNotFoundException(Guid participantId) : AppException("err_ParticipantNotFound", participantId);
}
