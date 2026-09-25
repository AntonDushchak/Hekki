using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.UI.Messages;
using Hekki.UI.Services;
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
            OnDisposing();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        protected virtual void OnDisposing()
        {
        }

        protected async Task ExecuteSafeAsync(Func<Task> action, [CallerMemberName] string? source = null)
        {
            try
            {
                await action();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ShowError(Localizer.ForException(ex), ex, source);
            }
        }

        protected void ShowError(string message, Exception? exception = null, [CallerMemberName] string? source = null)
        {
            WeakReferenceMessenger.Default.Send(new AppErrorMessage(message, exception, $"{GetType().Name}.{source}"));
        }

        protected void ShowSuccess(string message)
        {
            WeakReferenceMessenger.Default.Send(new AppSuccessMessage(message));
        }

        protected void ShowInfo(string message)
        {
            WeakReferenceMessenger.Default.Send(new AppInfoMessage(message));
        }

        protected void ShowWarning(string message)
        {
            WeakReferenceMessenger.Default.Send(new AppWarningMessage(message));
        }

        protected void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
