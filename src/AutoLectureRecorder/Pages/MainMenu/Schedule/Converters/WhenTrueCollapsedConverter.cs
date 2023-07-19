using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule.Converters;

public class WhenTrueCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
