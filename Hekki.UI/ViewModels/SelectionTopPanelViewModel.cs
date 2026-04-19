namespace Hekki.UI.ViewModels
{
    public class SelectionTopPanelViewModel
    {
        public string Title => "Selection";
        public SelectionTopPanelViewModel()
        {
        }

        public override string ToString()
        {
            return Title;
        }
    }
    public class CreateRaceTopPanelViewModel
    {
        public string Title => "Create";
        public CreateRaceTopPanelViewModel()
        {
        }
    }

    public class RaceTopPanelViewModel
    {
        public string Title => "Race";
        public RaceTopPanelViewModel()
        {
        }
    }

    public class TopPanelViewModel
    {
        public object LeftTopPanelContent { get; set; }
    }
}
