using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public partial class HeatGroupViewModel : ObservableObject
    {
        public readonly int GroupId;
        public HeatViewModel Heat { get; }

        public HeatGroupViewModel(int groupId, HeatViewModel heat)
        {
            GroupId = groupId;
            Heat = heat;
        }

        [ObservableProperty]
        private int _groupNumber = 3;

        [ObservableProperty]
        private int _groupCapacity = 8;

        public ObservableCollection<HeatRowViewModel> Rows { get; set; } = [];
    }
}
