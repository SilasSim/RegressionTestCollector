using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollector.Models
{
  public class DebugSessionInformation
  {
    public DebugSessionInformation(string path, RegressionTestViewModel test)
    {
      Path = path;
      Test = test;
    }
    public string Path { get; }

    public RegressionTestViewModel Test { get; }
  }
}
