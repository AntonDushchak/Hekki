using Hekki.UI.ViewModels;
using System.Globalization;
using System.Windows.Data;

namespace Hekki.UI.Converters
{
    public class ColumnHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ColumnViewModel column)
                return string.Empty;

            if (column.HeaderResourceKey is string resourceKey && App.Current.TryFindResource(resourceKey) is object resource)
            {
                return resource.ToString() ?? string.Empty;
            }

            return column.HeaderText ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
