using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoLectureRecorder.Resources.ResourceDictionaries.MainMenu.ScheduleView.Converters;

public class TimeRangeConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
            return null;

        var startTime = (TimeOnly)values[0];
        var endTime = (TimeOnly)values[1];
        
        var formattedStartTime = startTime.ToShortTimeString();
        var formattedEndTime = endTime.ToShortTimeString();

        return $"{formattedStartTime} - {formattedEndTime}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
