using System.ComponentModel;

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

    public class TopPanelViewModel : INotifyPropertyChanged
    {
        private object _leftTopPanelContent;

        public object LeftTopPanelContent
        {
            get => _leftTopPanelContent;
            set
            {
                _leftTopPanelContent = value;
                OnPropertyChanged(nameof(LeftTopPanelContent));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
