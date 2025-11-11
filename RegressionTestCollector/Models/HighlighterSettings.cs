using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Properties;

namespace RegressionTestCollector.Models
{
  public class HighlighterSettings
  {
    public bool IsTurnedOff
    {
      get => Settings.Default.HighlighterIsTurnedOff;
      set
      {
        Settings.Default.HighlighterIsTurnedOff = value;
        Settings.Default.Save();
      }
    }
  }
}
