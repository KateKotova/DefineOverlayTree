using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DefineOverlayTree.Converters
{
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string s && s == "Hide")
                {
                    return boolValue ? Visibility.Hidden : Visibility.Visible;
                }

                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
