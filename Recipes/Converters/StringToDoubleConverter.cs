using System.Globalization;
using System.Windows.Data;

namespace Recipes.Converters;
public class StringToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? string.Empty;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            stringValue = stringValue.Replace(",", ".");

            if (stringValue.Count(c => c == '.') == 1)
            {
                return stringValue;
            }

            if (double.TryParse(stringValue, NumberStyles.AllowDecimalPoint, culture, out var result))
            {
                return result;
            }
        }

        return null;
    }
}
