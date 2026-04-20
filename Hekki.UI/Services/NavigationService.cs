namespace Hekki.UI.Services;

using System;

public class NavigationService
{
    public Action<object> Navigate { get; set; }

    public void Go(object vm)
    {
        Navigate?.Invoke(vm);
    }
}
