using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.UI.Enums;
using CommunityToolkit.Mvvm.Messaging;
using System.Runtime.CompilerServices;

namespace Hekki.UI.ViewModels
{
    public abstract partial class ViewModelBase : ObservableValidator
    {
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
    }
}
