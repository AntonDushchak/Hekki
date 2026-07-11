namespace Hekki.Application.Exceptions
{
    public class RaceNotFoundException(int raceId) : Exception($"Гонка с id={raceId} не найдена");
    public class RegulationNotFoundException(int regulationId) : Exception($"Регламент с id={regulationId} не найден");
    public class HeatNotFoundException(int heatId) : Exception($"Хит с id={heatId} не найден");
}
