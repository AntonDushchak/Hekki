using CommunityToolkit.Mvvm.Input;

namespace Hekki.UI.ViewModels
{
    public class CellViewModel
    {
        public ColumnViewModel Column { get; }

        public bool IsEditable { get; init; }
        public ICellValue Value { get; private set; }

        public IRelayCommand? SaveCommand { get; init; }

        public CellViewModel(ColumnViewModel column, ICellValue value, bool isEditable = false)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            IsEditable = isEditable;
        }

        public void SetValue(ICellValue value)
        {
            Value = value;
        }
    }
}
