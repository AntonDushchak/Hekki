namespace Hekki.Application.Exceptions
{
    public class RaceNotFoundException(int raceId) : AppException("err_RaceNotFound", raceId);
}
