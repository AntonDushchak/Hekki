namespace Hekki.UI.Services;

public class ShellNavigationService : NavigationServiceBase
{
    private readonly PageFactory _pageFactory;

    public ShellNavigationService(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
    }

    public void Navigate(Uri uri)
    {
        var page = _pageFactory.CreatePage(uri);
        if (page is null) return;

        Frame?.Navigate(page);
    }

    public void NavigateToRace(int regulationId)
    {
        var page = _pageFactory.CreateRacePage(regulationId);
        if (page is null) return;

        Frame?.Navigate(page);
    }
}
