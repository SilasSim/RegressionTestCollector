using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using RegressionTestCollector.Models;
using RegressionTestCollector.Parser;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.CollectingStrategies
{
    /// <summary>
    /// Collection strategy directly implemented in CSharp
    /// </summary>
    public class CSharpCollectingStrategy : ICollectingStrategy
  {
    public event EventHandler<LoadingProgressEventArgs>? ProgressChanged;
    public CSharpCollectingStrategy(EventHandler<LoadingProgressEventArgs> onProgressChanged)
    {
      ProgressChanged += onProgressChanged;
    }

    public LoadingProgress? Progress { get; private set; }
    public string BatFilePattern { get; set; } = "RegressionTest*.bat";
    public string PythonScriptPattern { get; set; } = "RegTest*.py";
    public string PythonCommandVersion { get; set; } = "python";
    private RegressionTestParser mParser = new();
    public RegressionTestCollectionData Collect(string pythonCommand)
    {
      PythonCommandVersion = pythonCommand;
      Progress = new LoadingProgress(0, 100);
      OnProgressChanged();
      var result = new RegressionTestCollectionData();


      var openDirDialog = new OpenFolderDialog();
      if (openDirDialog.ShowDialog() == true)
      {
        var folderPath = openDirDialog.FolderName;
        var testBatFiles = FindFiles(folderPath, BatFilePattern);
        Progress.Max = testBatFiles.Length;
        OnProgressChanged();
        foreach (var batFile in testBatFiles)
        {
          var data = ProcessTestGroup(batFile);
          result.Data.AddRange(data.Data);
          result.Errors.AddRange(data.Errors);
          Progress.Current++;
          OnProgressChanged();
        }
      }

      return result;
    }

    /// <summary>
    /// Searches files with given pattern in the directory including subdirectories
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    private static string[] FindFiles(string folderPath, string pattern)
    {
      return Directory.GetFiles(folderPath, pattern, SearchOption.AllDirectories);
    }

    /// <summary>
    /// Event that fires when progress changes
    /// </summary>
    private void OnProgressChanged()
    {
      if (Progress is not null)
      {
        ProgressChanged?.Invoke(this, new LoadingProgressEventArgs(Progress));
      }
    }

    /// <summary>
    /// Processes the information in a given Regression Test bat file.
    /// </summary>
    /// <param name="path">Path to bat file</param>
    /// <returns><see cref="RegressionTestCollectionData"/></returns>
    private RegressionTestCollectionData ProcessTestGroup(string path)
    {
      var result = new RegressionTestCollectionData();
      var directory = Path.GetDirectoryName(path) ?? "";
      Dictionary<string, string> batArguments = mParser.BatParser.Parse(File.ReadAllText(path));

      var missingBatArguments = CheckForMissingBatArguments(batArguments);
      if (missingBatArguments.Length > 0)
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.ErrorParsingBatFile}: {String.Join($", ", missingBatArguments)} - {path}"));
        return result;
      }

      var definitionFilePath = File.ReadAllText(Path.Join(directory, batArguments["def"]));

      RegressionTestDefinitionDataObject? definitionData;
      // Definition
      try
      {
        definitionData = mParser.DefinitionParser.Parse(definitionFilePath);
        definitionData.ServerDefaultValue = definitionData.ServerDefaultValue
          .Replace("%EXETARGET;", batArguments["exeTarget"])
          .Replace("%ROOTDIR;", batArguments["rootDir"]);
      }
      catch (Exception ex)
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.ErrorParsingDefinitionFile} - {definitionFilePath}", ex));
        return result;
      }

      // Python
      var pythonScriptPaths = FindFiles(directory, PythonScriptPattern);
      if (pythonScriptPaths.Length == 0)
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.NoPythonScript} {directory}"));
        return result;
      }

      var pythonScriptCopy = PythonHelper.CreateAndTransformCopy(
        pythonScriptPaths[0], "print(command)", 
        ["subprocess.Popen(command)", "subprocess.run(command)"],
        [@"def\s+CreateInputCopyAndAdjustInput\s*\([^)]*\):\s*(?:\n[ \t]+.*)+",
          "CreateInputCopyAndAdjustInput()",
          "os.remove(args.i)"]
        );

      if (string.IsNullOrEmpty(pythonScriptCopy))
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.ErrorCreatingAndTransformingPythonCopy} - {directory}"));
        return result;
      }

      // ConfigData
      var configPath = Path.Join(directory, batArguments["useConfig"]);
      var configData = File.ReadAllText(configPath);
      RegressionTestConfigDataObject configDataObject;
      try
      {
        configDataObject = mParser.ConfigParser.Parse(configData, batArguments["rootDir"]);
      }
      catch (Exception ex)
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.ErrorParsingConfigFile} - {configPath}", ex));
        return result;
      }

      try
      {
        // running python tests
        foreach (var regressionTest in configDataObject.RegressionTests)
        {
          Debug.Write(".");

          ProcessStartInfo startInfo = new ProcessStartInfo()
          {
            FileName = PythonCommandVersion,
            Arguments =
              $"\"{pythonScriptCopy}\" {regressionTest.GetCommandStringForPythonScript()} -server \"{definitionData.ServerDefaultValue}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
          };

          var outString = PythonHelper.RunProcess(startInfo);
          FormatOutputString(outString);

          var folderPath = Path.GetDirectoryName(path) ?? string.Empty;
          result.Data.Add(new RegressionTestDataObject(regressionTest.Name, definitionData.Name,
            FormatOutputString(outString), 
            folderPath, batArguments["rootDir"].Replace(@"/", @"\").Replace(@"\\", @"\"), pythonScriptPaths[0])
          {
            InputFile = regressionTest.Sourcefile,
            OutputFile = regressionTest.Outfile,
            Scenario = StringUtils.GetArgumentFromString(regressionTest.GetCommandStringForPythonScript(), "scenario")
          });
        }
      }
      catch (Exception ex)
      {
        result.Errors.Add(new ErrorObject($"{Resources.CollectingErrors.ErrorRunningPythonCopy} - {pythonScriptCopy}", ex));
        return result;
      }

      File.Delete(pythonScriptCopy);
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="batArguments"></param>
    /// <returns>The names of the missing arguments</returns>
    internal static string[] CheckForMissingBatArguments(Dictionary<string, string> batArguments)
    {
      List<string> errorStrings = [];

      string[] arguments = ["def", "exeTarget", "rootDir", "useConfig"];

      foreach (var arg in arguments)
      {
        if (!batArguments.ContainsKey(arg))
        {
          errorStrings.Add(arg);
        }
      }

      return errorStrings.ToArray();
    }

    /// <summary>
    /// Formats the output string that is in array format to a valid windows command string
    /// </summary>
    /// <param name="outString"></param>
    /// <returns></returns>
    internal static string FormatOutputString(string outString)
    {
      outString = outString.Trim();
      if (outString[0] != '[' && outString[^1] != ']')
      {
        return "";
      }

      outString = outString.Replace('[', ' ');
      outString = outString.Replace(']', ' ');
      outString = outString.Replace('\'', ' ');
      outString = outString.Replace("/", @"\");
      outString = outString.Replace(@"\\", @"\");

      var arguments = outString.Split(",");

      StringBuilder stringBuilder = new StringBuilder();
      foreach (var argument in arguments)
      {
        var argumentString = argument.Trim();


        if (argumentString.StartsWith("\\"))
        {
          argumentString = "-" + argumentString[1..];
        }


        if (argumentString.Contains(' '))
        {
          argumentString = $"\"{argumentString}\"";
        }

        stringBuilder.Append(argumentString + ' ');
      }

      return stringBuilder.ToString().Trim();
    }
  }
}
