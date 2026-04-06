using Hekki.UI.Pages;
using Hekki.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Hekki.UI
{
    public class NavigationService
    {
        private readonly Dictionary<string, Page> _pageCache = [];
        private readonly IServiceProvider _services;

        public NavigationService(IServiceProvider services)
        {
            _services = services;
        }

        public Page? CreatePage(Uri uri)
        {
            return uri.ToString() switch
            {
                "/Pages/RegulationSelection.xaml" => CreateRegulationSelectionPage(),
                "/Pages/Race.xaml" => _pageCache.GetValueOrDefault("/Pages/Race.xaml")
                      ?? AddToCache("/Pages/Race.xaml", new Race()),
                "/Pages/Preference.xaml" => new Preference(), // без DI
                _ => null
            };
        }

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
}
