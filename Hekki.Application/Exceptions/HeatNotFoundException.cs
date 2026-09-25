namespace Hekki.Application.Exceptions
{
    public class HeatNotFoundException(int heatId) : AppException("err_HeatNotFound", heatId);
}
