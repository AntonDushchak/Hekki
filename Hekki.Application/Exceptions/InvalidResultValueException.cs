namespace Hekki.Application.Exceptions
{
    public class InvalidResultValueException(long value) : AppException("err_InvalidResultValue", value);
}
