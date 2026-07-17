using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class TopPanelViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;

        public RegulationPickerViewModel RegulationPicker { get; }

        public TopPanelViewModel(RegulationPickerViewModel regulationPicker)
        {
            RegulationPicker = regulationPicker;
        }
    }
}
