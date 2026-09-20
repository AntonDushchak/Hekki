using Hekki.UI.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hekki.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public static BoolToVisibilityConverter Instance { get; } = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter?.ToString() == "Inverse")
                {
                    return !boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                if (parameter?.ToString() == "Inverse")
                {
                    return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            bool isInverse = parameter?.ToString() == "Inverse";

            if (isInverse)
            {
                return isNull ? Visibility.Visible : Visibility.Collapsed;
            }

            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            bool isInverse = parameter?.ToString() == "Inverse";

            if (isInverse)
            {
                return isNull ? true : false;
            }

            return isNull ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

    public class StringToVisibilityConverter : IValueConverter
    {
        public static StringToVisibilityConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
