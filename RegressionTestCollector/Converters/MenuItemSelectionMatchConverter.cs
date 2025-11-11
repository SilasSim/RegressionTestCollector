using System.Globalization;
using System.Windows.Data;

namespace RegressionTestCollector.Converters
{
  public class MenuItemSelectionMatchConverter : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (string.IsNullOrWhiteSpace(value as string) || string.IsNullOrWhiteSpace(parameter as string))
      {
        return false;
      }

      if (value.Equals(parameter))
      {
        return true;
      }

      return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value is bool isChecked && isChecked && parameter is string paramString)
      {
        return paramString;
      }

      return Binding.DoNothing;
    }
  }
}
