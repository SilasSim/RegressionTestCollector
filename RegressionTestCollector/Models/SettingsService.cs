using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Properties;

namespace RegressionTestCollector.Models
{
  public interface ISettingsService
  {
    string SelectedTheme { get; set; }
    string SelectedStrategy { get; set; }
    string SelectedPythonCommand { get; set; }

    bool IsUsingAbsolutePath { get; set; }

    bool IsWindows { get; set; }

    bool IsDebugSet { get; set; }
    bool IsVerboseSet { get; set; }

    bool IsExeIncluded { get; set; }

    HighlighterSettings HighlighterSettings { get; }
  }

  public class SettingsService : ISettingsService
  {
    public string SelectedTheme
    {
      get => Settings.Default.SelectedTheme;
      set
      {
        Settings.Default.SelectedTheme = value;
        Save();
      }
    }

    public string SelectedStrategy
    {
      get => Settings.Default.SelectedCollectingMethod;
      set
      {
        Settings.Default.SelectedCollectingMethod = value;
        Save();
      } 
    }
    public string SelectedPythonCommand
    {
      get => Settings.Default.SelectedPythonCommand;
      set
      {
        Settings.Default.SelectedPythonCommand = value;
        Save();
      }
    }

    public bool IsUsingAbsolutePath
    {
      get => Settings.Default.CommandStringIsUsingAbsolutePath;
      set
      {
        Settings.Default.CommandStringIsUsingAbsolutePath = value;
        Save();
      }
    }
    public bool IsWindows
    {
      get => Settings.Default.CommandStringSettigsIsWindows;
      set
      {
        Settings.Default.CommandStringSettigsIsWindows = value;
        Save();
      }
    }
    public bool IsDebugSet
    {
      get => Settings.Default.CommandStringSettingsIsDebugSet;
      set
      {
        Settings.Default.CommandStringSettingsIsDebugSet = value;
        Save();
      }
    }
    public bool IsVerboseSet
    {
      get => Settings.Default.CommandStringSettingsIsVerboseSet;
      set
      {
        Settings.Default.CommandStringSettingsIsVerboseSet = value;
        Save();
      }
    }
    public bool IsExeIncluded
    {
      get => Settings.Default.CommandStringSettingsIsExeIncluded;
      set
      {
        Settings.Default.CommandStringSettingsIsExeIncluded = value;
        Save();
      }
    }

    private readonly HighlighterSettings _highlighterSettings = new HighlighterSettings();
    public HighlighterSettings HighlighterSettings
    {
      get => _highlighterSettings;
    }

    public void Save()
    {
      Settings.Default.Save();
    }
  }
}
