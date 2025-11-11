using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTestCollector.Utils
{
  public class PropertyValueChangedEventArgs : EventArgs
  {
    public string PropertyName { get; }
    public object? NewValue { get; }

    public PropertyValueChangedEventArgs(string propertyName, object? newValue)
    {
      PropertyName = propertyName;
      NewValue = newValue;
    }
  }
}
