namespace Hekki.UI.Services;

public class RootNavigationService : NavigationServiceBase
{

    private readonly PageFactory _pageFactory;

    public RootNavigationService(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
    }

    public void Navigate(Uri uri)
    {
        var page = _pageFactory.CreatePage(uri);
        if (page is null) return;

        Frame?.Navigate(page);
    }
}
