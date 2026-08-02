namespace Hekki.Application.Exceptions
{
    public class RaceNotFoundException(int raceId) : Exception($"Гонка с id={raceId} не найдена");
    public class RegulationNotFoundException(int regulationId) : Exception($"Регламент с id={regulationId} не найден");
    public class HeatNotFoundException(int heatId) : Exception($"Хит с id={heatId} не найден");
    public class EntityNotFoundException(string entityName, int entityId) : Exception($"{entityName} with id={entityId} is not found.");
    public class EntityNotFoundWithException(string entityName, string withEntityName, int withEntityId) : Exception($"{entityName} with {withEntityName} id={withEntityId} is not found.");

}
