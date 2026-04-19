namespace Hekki.UI.ViewModels
{
    public class MainViewModel
    {
        public TopPanelViewModel TopPanelVM { get; }
        public object CurrentPageVM { get; set; }

        public MainViewModel()
        {
            TopPanelVM = new TopPanelViewModel();
            NavigateToSelection();
        }

        public void NavigateToRace()
        {
            var vm = new RaceViewModel();

            CurrentPageVM = vm;
            TopPanelVM.LeftTopPanelContent = new RaceTopPanelViewModel();
        }

        public void NavigateToCreate()
        {
            var vm = new CreateRaceViewModel();

            CurrentPageVM = vm;
            TopPanelVM.LeftTopPanelContent = new CreateRaceTopPanelViewModel();
        }

        public void NavigateToSelection()
        {
            var vm = new SelectionViewModel();

            CurrentPageVM = vm;
            TopPanelVM.LeftTopPanelContent = new SelectionTopPanelViewModel();
        }
    }

    
}
