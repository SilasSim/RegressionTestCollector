using System.Globalization;
using System.Windows.Data;

namespace RegressionTestCollector.Converters
{
  public class InverseBooleanConverter : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      var b = (bool)(value ?? throw new ArgumentNullException(nameof(value)));
      return !b;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      var b = (bool)(value ?? throw new ArgumentNullException(nameof(value)));
      return !b;
    }
  }
}
