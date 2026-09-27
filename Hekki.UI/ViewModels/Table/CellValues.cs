using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.UI.Services;
using System.Collections;
using System.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public interface ICellValue
    {
    }

    public enum CellInputKind
    {
        Text,
        Integer,
        Time
    }

    public partial class TextCellValue : ObservableObject, ICellValue, INotifyDataErrorInfo
    {
        private readonly Func<string, string?>? _validate;
        private string? _error;

        [ObservableProperty]
        private string _text;

        public CellInputKind InputKind { get; }

        public TextCellValue(string text, CellInputKind inputKind = CellInputKind.Text, Func<string, string?>? validate = null)
        {
            _text = text;
            InputKind = inputKind;
            _validate = validate;
        }

        public bool HasErrors => _error != null;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return propertyName == nameof(Text) && _error != null ? new[] { _error } : Array.Empty<string>();
        }

        partial void OnTextChanged(string value)
        {
            var errorKey = _validate?.Invoke(value);
            var error = errorKey == null ? null : Localizer.Get(errorKey);
            if (error == _error) return;

            _error = error;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
            OnPropertyChanged(nameof(HasErrors));
        }
    }

    public class ParticipantCellValue : ICellValue
    {
        public string Name { get; init; } = "";
        public string KartNumbers { get; init; } = "";
    }
}
