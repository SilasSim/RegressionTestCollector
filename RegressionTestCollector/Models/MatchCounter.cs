using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMArchitecture;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.Models
{
  public class MatchCounter : ObservableObject
  {
    public MatchCounter(int visibleCounter, int totalCounter)
    {
      VisibleCounter = visibleCounter;
      TotalCounter = totalCounter;
    }

    private int mVisibleCounter;
    public int VisibleCounter
    {
      get => mVisibleCounter;
      set
      {
        if (SetField(ref mVisibleCounter, value))
        {
          OnPropertyChanged(nameof(MatchCounterText));
        }
      }
    }

    private int mTotalCounter;
    public int TotalCounter
    {
      get => mTotalCounter;
      set
      {
        if (SetField(ref mTotalCounter, value))
        {
          OnPropertyChanged(nameof(MatchCounterText));
        }
      }
    }

    public string MatchCounterText => $"{VisibleCounter} / {TotalCounter}";
  }
}
