namespace Hekki.Application.Exceptions
{
    public class HeatNotDrawnException(int heatNumber) : AppException("err_HeatNotDrawn", heatNumber);
}
