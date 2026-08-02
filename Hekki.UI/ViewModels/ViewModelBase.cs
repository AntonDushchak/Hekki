using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.UI.Messages.App;
using System.Runtime.CompilerServices;

namespace Hekki.UI.ViewModels
{
    public abstract partial class ViewModelBase : ObservableValidator, IDisposable
    {
        protected ViewModelBase()
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        protected async Task ExecuteSafeAsync(Func<Task> action, [CallerMemberName] string? source = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new AppErrorMessage(ex.Message));
            }
        }

        protected void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
