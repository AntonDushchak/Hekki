namespace Hekki.Application.Abstractions
{
    public interface IEventPublisher
    {
        void Publish<T>(T message) where T : class;
    }
}