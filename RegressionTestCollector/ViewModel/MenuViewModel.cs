using RegressionTestCollector.Models;
using RegressionTestCollector.Properties;
using RegressionTestCollector.Themes;

namespace RegressionTestCollector.ViewModel
{
  public class MenuViewModel : ViewModelBase
  {
    private IThemeService mThemeService;
    private ISettingsService mSettingsService;

    public MenuViewModel(ISettingsService settingsService, IThemeService themeService)
    {
      mThemeService = themeService;
      mSettingsService = settingsService;

      mSelectedTheme = mSettingsService.SelectedTheme;
      mSelectedStrategy = mSettingsService.SelectedStrategy;
      mSelectedPythonCommand = mSettingsService.SelectedPythonCommand;
    }

    public void InitializeTheme()
    {
      mThemeService.ApplyTheme(mSelectedTheme);
    }

    private string mSelectedPythonCommand;

    public string SelectedPythonCommand
    {
      get => mSelectedPythonCommand;
      set
      {
        if (SetField(ref mSelectedPythonCommand, value))
        {
          mSettingsService.SelectedPythonCommand = value;
        }
      }
    }

    private string mSelectedTheme;

    public string SelectedTheme
    {
      get => mSelectedTheme;
      set
      {
        if (SetField(ref mSelectedTheme, value))
        {
          mSettingsService.SelectedTheme = value;
          mThemeService.ApplyTheme(value);
        }
      }
    }

    private string mSelectedStrategy;

    public string SelectedStrategy
    {
      get => mSelectedStrategy;
      set
      {
        if (SetField(ref mSelectedStrategy, value))
        {
          mSettingsService.SelectedStrategy = value;
        }
      }
    }
  }
}
