namespace Hekki.UI.Services;

using System;

public class NavigationService : INavigationService
{
    public Action<object> Navigate { get; set; }

    public void Go(object vm)
    {
        Navigate?.Invoke(vm);
    }
}

public interface INavigationService
{
    Action<object> Navigate { get; set; }
    void Go(object vm);
}
