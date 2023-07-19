using System.Globalization;
using System.Windows;
using System;
using System.Windows.Data;

namespace AutoLectureRecorder.Resources.ResourceDictionaries.MainMenu.ScheduleView.Converters;

public class WhenTrueVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
