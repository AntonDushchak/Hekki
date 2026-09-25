using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstractions;
using Hekki.UI.Messages;

namespace Hekki.UI.Services
{
    public class MessengerEventPublisher : IEventPublisher
    {
        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            try
            {
                WeakReferenceMessenger.Default.Send(message);
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(Localizer.ForException(ex), ex, $"Receive({typeof(TMessage).Name})"));
            }
        }
    }
}
