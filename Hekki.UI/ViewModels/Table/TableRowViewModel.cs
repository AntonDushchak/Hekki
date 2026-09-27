using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public abstract class TableRowViewModel : ObservableObject
    {
        public List<CellViewModel> Cells { get; } = [];
    }
}
