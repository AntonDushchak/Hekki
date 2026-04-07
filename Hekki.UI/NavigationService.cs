using Hekki.UI.Pages;
using Hekki.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Hekki.UI;

public class NavigationService
{
    private readonly Dictionary<string, Page> _pageCache = [];
    private readonly IServiceProvider _services;

    private Frame? _frame;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public void SetFrame(Frame frame) => _frame = frame;

    public void Navigate(Uri uri)
    {
        var page = CreatePage(uri);
        if (page is null) return;

        _frame?.Navigate(page);

        CleanHistory();
    }

    public void Navigate(Page page)
    {
        _frame?.Navigate(page);
        CleanHistory();
    }

    public void CleanHistory()
    {
        while (_frame?.CanGoBack == true) _frame.RemoveBackEntry();
    }   

    public void NavigateToRace(int regulationId)
    {
        var key = $"/Pages/Race.xaml?reg={regulationId}";
        var page = _pageCache.GetValueOrDefault(key)
                   ?? AddToCache(key, CreateRacePage(regulationId));

        _frame?.Navigate(page);
        //CleanHistory();
    }

    public Page? CreatePage(Uri uri)
    {
        return uri.ToString() switch
        {
            "/Pages/RegulationSelection.xaml" => CreateRegulationSelectionPage(),
            "/Pages/RegulationCreation.xaml" => _pageCache.GetValueOrDefault("/Pages/RegulationCreation.xaml")
                  ?? AddToCache("/Pages/RegulationCreation.xaml", CreateRegulationCreationPage()),
            "/Pages/Preference.xaml" => new Preference(),
            _ => null
        };
    }

    private RegulationCreation CreateRegulationCreationPage()
    {
        return new();
    }

    private Page CreateRacePage(int regulationId)
        => ActivatorUtilities.CreateInstance<Race>(_services, regulationId);

    private Page AddToCache(string key, Page page)
    {
        _pageCache[key] = page;
        return page;
    }

    private RegulationSelection CreateRegulationSelectionPage()
    {
        var viewModel = _services.GetRequiredService<RegulationSelectionViewModel>();
        return new RegulationSelection(viewModel);
    }
}