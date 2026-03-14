using System;
using System.Windows;
using System.Windows.Data;

namespace DefineOverlayTree.Converters
{
    public class TrimmedContentToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is FrameworkElement control)) return Visibility.Hidden;

            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return control.ActualWidth < control.DesiredSize.Width ? Visibility.Visible : Visibility.Hidden;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
