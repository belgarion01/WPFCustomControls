using System;
using System.Globalization;
using System.Windows.Data;

namespace CustomControls.Converters;

public class IntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int integer)
        {
            return integer.ToString();
        }
        
        throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            if (int.TryParse(stringValue, out int integer))
            {
                return integer;
            }
        }

        return 0;
    }
}