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
        if (double.TryParse(value?.ToString(), out var result))
        {
            return result;
        }

        return null;
    }
}
