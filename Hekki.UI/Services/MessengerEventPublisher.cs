using CommunityToolkit.Mvvm.Messaging;
using Hekki.Application.Abstrations;
using Hekki.Application.Messages;

namespace Hekki.UI.Services
{
    public class MessengerEventPublisher : IEventPublisher
    {
        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            // Ошибка в обработчике UI не должна выглядеть как ошибка сервиса,
            // который к этому моменту уже успешно отработал.
            try
            {
                WeakReferenceMessenger.Default.Send(message);
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(ex.Message, ex, $"Receive({typeof(TMessage).Name})"));
            }
        }
    }
}
