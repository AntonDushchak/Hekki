using Hekki.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Hekki.UI.Services;

public class PageFactory
{
    private readonly Dictionary<string, Page> _pageCache = new Dictionary<string, Page>();
    private readonly IServiceProvider _services;
    public PageFactory(IServiceProvider services)
    {
        _services = services;
    }
    public Page? CreatePage(Uri uri)
    {
        return CreatePage(uri.ToString());
    }

    public Page? CreatePage(string uri)
    {
        if (_pageCache.TryGetValue(uri, out var page))
            return page;

        return uri switch
        {
            "/Pages/RegulationShell.xaml" => AddToCache(uri, ActivatorUtilities.CreateInstance<RegulationShell>(_services)),
            "/Pages/Preference.xaml" => new Preference(),
            "/Pages/RegulationCreation.xaml" => AddToCache(uri, ActivatorUtilities.CreateInstance<RegulationCreation>(_services)),
            _ => null
        };
    }

    public Page CreateRacePage(int regulationId)
    {
        var key = $"/Pages/Race.xaml?reg={regulationId}";
        if (_pageCache.TryGetValue(key, out var page))
            return page;

        page = ActivatorUtilities.CreateInstance<Race>(_services, regulationId);
        return AddToCache(key, page);
    }

    private Page AddToCache(string key, Page page)
    {
        _pageCache[key] = page;
        return page;
    }
}