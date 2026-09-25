namespace Hekki.Application.Exceptions
{
    public class EntityNotFoundException(string entityName, int entityId) : AppException("err_EntityNotFound", entityName, entityId);
    public class EntityNotFoundWithException(string entityName, string withEntityName, int withEntityId) : AppException("err_EntityNotFoundWith", entityName, withEntityName, withEntityId);
}
