namespace Hekki.Application.Abstrations
{
    public interface IEventPublisher
    {
        void Publish<T>(T message) where T : class;
    }
}