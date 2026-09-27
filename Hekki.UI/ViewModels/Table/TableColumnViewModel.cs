using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public abstract partial class TableColumnViewModel : ObservableObject
    {
        [ObservableProperty]
        private double _columnWidth;

        protected TableColumnViewModel(double columnWidth)
        {
            _columnWidth = columnWidth;
        }

        public virtual string? HeaderResourceKey => null;
        public virtual string? HeaderText => null;
        public virtual bool IsNumeric => false;
        public virtual CellInputKind InputKind => CellInputKind.Text;

        public virtual string? ValidateInput(string text) => null;

        protected TextCellValue CreateTextValue(string text) => new(text, InputKind, ValidateInput);
    }
}
