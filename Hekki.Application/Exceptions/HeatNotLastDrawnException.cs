namespace Hekki.Application.Exceptions
{
    public class HeatNotLastDrawnException(int heatNumber) : AppException("err_HeatNotLastDrawn", heatNumber);
}
