using System;
using System.Globalization;
using System.Windows.Data;

namespace CustomControls.Converters;

public class DoubleToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d.ToString();
        }
        
        throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            if (double.TryParse(stringValue, out double d))
            {
                return d;
            }
        }

        return 0.0;
    }
}