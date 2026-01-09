using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MVVMArchitecture;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.Models
{
  public class Highlighter : ObservableObject
  {
    public EventHandler<PropertyValueChangedEventArgs>? PropertyValueChanged;
    private bool mIsTurnedOff;

    public bool IsTurnedOff
    {
      get => mIsTurnedOff;
      set => SetField(ref mIsTurnedOff, value);
    }

    protected new bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;

      field = value;
      OnPropertyChanged(propertyName);
      PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(propertyName, value));
      return true;
    }
  }
}
