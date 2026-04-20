using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public class CreateRaceViewModel
    {
        private readonly NavigationService navigationService;
        private readonly IViewModelFactory viewModelFactory;

        public CreateRaceViewModel(NavigationService navigationService, IViewModelFactory viewModelFactory)
        {
            this.navigationService = navigationService;
            this.viewModelFactory = viewModelFactory;
        }
    }
}