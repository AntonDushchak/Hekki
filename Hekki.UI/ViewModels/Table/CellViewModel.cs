using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class CellViewModel : ObservableObject
    {
        public TableColumnViewModel Column { get; }

        public bool IsEditable { get; init; }

        [ObservableProperty]
        private ICellValue _value;

        public CellViewModel(TableColumnViewModel column, ICellValue value, bool isEditable = false)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            _value = value ?? throw new ArgumentNullException(nameof(value));
            IsEditable = isEditable;
        }

        public void SetValue(ICellValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
