namespace Hekki.Application.Exceptions
{
    public class HeatAlreadyDrawnException(int heatNumber) : AppException("err_HeatAlreadyDrawn", heatNumber);
}
