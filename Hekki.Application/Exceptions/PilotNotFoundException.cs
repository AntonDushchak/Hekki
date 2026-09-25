namespace Hekki.Application.Exceptions
{
    public class PilotNotFoundException(int pilotId) : AppException("err_PilotNotFound", pilotId);
}
