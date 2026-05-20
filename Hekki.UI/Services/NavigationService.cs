namespace Hekki.UI.Services;

using System;
using Hekki.UI.ViewModels;

public interface INavigationService
{
    event Action<object>? Navigated;

    void NavigateToSelection();
    void NavigateToCreateRace();
    void NavigateToRace(int regulationId);
}

public class NavigationService : INavigationService
{
    private readonly IViewModelFactory _viewModelFactory;

    public event Action<object>? Navigated;

    public NavigationService(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateToSelection()
    {
        var vm = _viewModelFactory.Create<SelectionViewModel>();
        Navigated?.Invoke(vm);
    }

    public void NavigateToCreateRace()
    {
        var vm = _viewModelFactory.Create<CreateRaceViewModel>();
        Navigated?.Invoke(vm);
    }

    public void NavigateToRace(int regulationId)
    {
        var vm = _viewModelFactory.CreateRaceViewModel(regulationId);
        Navigated?.Invoke(vm);
    }
}
