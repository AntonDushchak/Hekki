using Hekki.UI.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hekki.UI.Converters
{
    public class SettingsTypeToVisibilityConverter : IValueConverter
    {
        public static SettingsTypeToVisibilityConverter Instance { get; } = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MethodSettingsType settingsType && parameter is string paramStr)
            {
                if (Enum.TryParse<MethodSettingsType>(paramStr, out var paramEnum))
                {
                    return settingsType == paramEnum ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
