using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTestCollector.ViewModel
{
  public class HelpViewModel : ViewModelBase
  {

    private string mHelpText = string.Empty;

    public string HelpText
    {
      get => mHelpText;
      set => SetField(ref mHelpText, value);
    }

    private bool mIsExeIncluded;

    public bool IsExeIncluded
    {
      get => mIsExeIncluded;
      set
      {
        if (SetField(ref mIsExeIncluded, value))
        {
          UpdateHelpText();
        }
      }
    }

    private bool mIsWindows;

    public bool IsWindows
    {
      get => mIsWindows;
      set
      {
        if (SetField(ref mIsWindows, value))
        {
          UpdateHelpText();
        }
      }
    }

    private bool mIsAbsolutePath;

    public bool IsAbsolutePath
    {
      get => mIsAbsolutePath;
      set
      {
        if (SetField(ref mIsAbsolutePath, value))
        {
          UpdateHelpText();
        }
      }
    }

    public string[] HttpServerSteps { get; } = Resources.Strings.HelpHttpServerSteps.Split('|');
    public string[] CommandStringBulletPoints { get; } = Resources.Strings.HelpCommandStringBulletPoints.Split('|');

    public string[] CollectingStrategyBulletPoints { get; } =
      Resources.Strings.HelpCollectingStrategyBulletPoints.Split('|');

    private void UpdateHelpText()
    {
      if (IsAbsolutePath && IsExeIncluded && IsWindows)
      {
        HelpText = "The command can be copied in a windows console to run the command";
      } 
      else if (IsAbsolutePath && IsExeIncluded && !IsWindows)
      {
        HelpText = "The paths are adjusted to fit the a with wsl builded path. The command can be copied anywhere in a linux/wsl shell to run the command after CANdelaStudio was builded with wsl. If you have separated repositories for linux and windows, you need to use the linux repository.";
      }
      else if (!IsAbsolutePath && IsExeIncluded && IsWindows)
      {
        HelpText = "The command can be executed in a console where the executable directory is the working directory.";
      }
      else if (IsAbsolutePath && !IsExeIncluded && IsWindows)
      {
        HelpText = "The command string can be copied in the debug settings of an IDE.";
      }
      else if (!IsAbsolutePath && !IsExeIncluded && IsWindows)
      {
        HelpText = "The command string can be copied in the debug settings of an IDE.";
      }
      else if (IsAbsolutePath && IsExeIncluded && IsWindows)
      {
        HelpText = "The command can be copied anywhere in a linux/wsl shell";
      }

      OnPropertyChanged(HelpText);
    }
  }
}
