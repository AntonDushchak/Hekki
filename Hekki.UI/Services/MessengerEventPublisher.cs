using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;

namespace Hekki.UI.Services
{
    public class MessengerEventPublisher : IEventPublisher
    {
        public void Publish<TMessage>(TMessage message) 
            where TMessage : class
        {
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
