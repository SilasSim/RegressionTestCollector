using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Properties;

namespace RegressionTestCollectorTests.Models
{
  public interface ISettingsService
  {
    string SelectedTheme { get; set; }
    string SelectedStrategy { get; set; }
    string SelectedPythonCommand { get; set; }
    void Save();
  }

  public class SettingsService : ISettingsService
  {
    public string SelectedTheme
    {
      get => Settings.Default.SelectedTheme;
      set => Settings.Default.SelectedTheme = value;
    }

    public string SelectedStrategy
    {
      get => Settings.Default.SelectedCollectingMethod;
      set => Settings.Default.SelectedCollectingMethod = value;
    }
    public string SelectedPythonCommand
    {
      get => Settings.Default.SelectedPythonCommand;
      set => Settings.Default.SelectedPythonCommand = value;
    }
    public void Save()
    {
      Settings.Default.Save();
    }
  }
}
