using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegressionTestCollector.Converters
{
  public class TextHighlightConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

      var text = values[0] as string;
      var inlines = new List<Inline>();
      if (string.IsNullOrEmpty(text))
      {
        return inlines;
      }

      string? keyword = values[1] as string;
      if (keyword is null)
      {
        return inlines;
      }

      int index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);

      if (index >= 0)
      {
        inlines.Add(new Run(text.Substring(0, index)));

        inlines.Add(new Run(text.Substring(index, keyword.Length))
        {
          Foreground = Brushes.Yellow
        });

        inlines.Add(new Run(text.Substring(index + keyword.Length)));

      }
      else
      {
        inlines.Add(new Run(text));
      }

      return inlines;

    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}
