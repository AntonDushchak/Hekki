namespace Hekki.Application.Exceptions
{
    public class RegulationNotFoundException(int regulationId) : AppException("err_RegulationNotFound", regulationId);
}
