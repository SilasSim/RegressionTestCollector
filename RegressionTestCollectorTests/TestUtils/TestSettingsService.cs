using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.TestUtils
{
  public class TestSettingsService : ISettingsService
  {
    public string SelectedTheme { get; set; } = "Light";
    public string SelectedStrategy { get; set; } = "CSharp";
    public string SelectedPythonCommand { get; set; } = "python";
    public bool IsUsingAbsolutePath { get; set; }
    public bool IsWindows { get; set; }
    public bool IsDebugSet { get; set; }
    public bool IsVerboseSet { get; set; }
    public bool IsExeIncluded { get; set; }
    public HighlighterSettings HighlighterSettings { get; } = new();
    public bool SaveCalled { get; private set; }
    public void Save() => SaveCalled = true;
  }
}
