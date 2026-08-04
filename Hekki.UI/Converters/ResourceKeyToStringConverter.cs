using System.Globalization;
using System.Windows.Data;

namespace Hekki.UI.Converters
{
    public class ResourceKeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            var key = value.ToString();
            if (key == null) return string.Empty;

            var res = App.Current.TryFindResource(key);
            return res?.ToString() ?? key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
