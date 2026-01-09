using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.Utils
{

  /// <summary>
  /// Helper class to run a python script and read its output
  /// </summary>
  public static class PythonHelper
  {
    public static string RunProcess(ProcessStartInfo processStartInformation, Action<string>? onOutput = null, Action<string>? onError = null)
    {
      var outputBuilder = new StringBuilder();
      var errorBuilder = new StringBuilder();

      using (var process = Process.Start(processStartInformation))
      {
        if (process == null)
        {
          return "";
        }

        process.BeginOutputReadLine();

        process.OutputDataReceived += (sender, args) =>
        {
          if (args.Data != null)
          {
            outputBuilder.Append(args.Data);
            onOutput?.Invoke(args.Data);
          }
        };

        process.BeginErrorReadLine();

        process.ErrorDataReceived += (sender, args) =>
        {
          if (args.Data != null)
          {
            errorBuilder.Append(args.Data);
            onError?.Invoke(args.Data);
          }
        };

        process.WaitForExit();
      }

      return outputBuilder.ToString();
    }

    public static string CreateDebugCopy(RegressionTestDataObject obj)
    {
      var debugScriptPath = obj.PythonScriptPath.Replace(".py", "_debugRTC.py");
      var stringBuilder = new StringBuilder();
      string newLine = String.Empty;

      var content = File.ReadAllLines(obj.PythonScriptPath);
      foreach (var line in content)
      {
        newLine = line;
        if (line.Contains("required=True"))
        {
          newLine = line.Replace("required=True", "required=False");
        }
        else if (line.Contains("ServerApi.StartServer"))
        {
          newLine = line.Replace("ServerApi.StartServer", "# ServerApi.StartServer");
        }
        else if (line.Contains("waitForServerToStart()"))
        {
          newLine = line.Replace("waitForServerToStart()", "# waitForServerToStart()");
        } 
        else if (line.Contains("parse_known_args"))
        {
          stringBuilder.AppendLine(newLine);
          int indentLength = line.TakeWhile(char.IsWhiteSpace).Count();
          string indentation = line.Substring(0, indentLength);

          var absoluteRootDir = Path.GetFullPath(obj.RootDir, obj.FolderPath);

          var argLine = $"{indentation}args.i = \"{obj.InputFile}\""
            .Replace(obj.RootDir.Replace(@"\", @"/"), absoluteRootDir)
            .Replace(@"\", @"\\")
            .Replace(@"/", @"\\");
          stringBuilder.AppendLine(argLine);

          argLine = $"{indentation}args.o = \"{obj.OutputFile}\""
            .Replace(obj.RootDir.Replace(@"\", @"/"), absoluteRootDir)
            .Replace(@"\", @"\\")
            .Replace(@"/", @"\\");
          stringBuilder.AppendLine(argLine);

          argLine = $"{indentation}args.scenario = \"{obj.Scenario}\"";
          stringBuilder.AppendLine(argLine);
          continue;
        }

        stringBuilder.AppendLine(newLine);
      }

      File.WriteAllText(debugScriptPath, stringBuilder.ToString());

      return debugScriptPath;
    }

    public static string ReadPythonOutputFile(string path)
    {
      return File.ReadAllText(path);
    }

    public static string GenerateAndReadPythonScriptOutput(string path, string pythonCommand, Action<string>? onOutput = null, Action<string>? onError = null)
    {
      var processStartInformation = new ProcessStartInfo()
      {
        FileName = pythonCommand,
        Arguments = path,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      RunProcess(processStartInformation, onOutput, onError);

      return ReadPythonOutputFile(path.Replace(".py", "Results.txt"));
    }

    /// <summary>
    /// Creates a copy of a given file and replaces strings with different string
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <param name="newString"></param>
    /// <param name="stringsToReplace">Strings to replace with the new string</param>
    /// <param name="patternsToRemove">Strings to remove. Regex can be used here.</param>
    /// <returns></returns>
    public static string CreateAndTransformCopy(string path, string newString, string[] stringsToReplace, string[] patternsToRemove)
    {
      if (!File.Exists(path))
      {
        return "";
      }

      var copyFileName = path.Replace(".py", "_copy.py");
      File.Copy(path, copyFileName, true);

      string content = File.ReadAllText(copyFileName);

      foreach (var s in stringsToReplace)
      {
        content = content.Replace(s, newString);
      }

      foreach (var s in patternsToRemove)
      {
        content = Regex.Replace(content, s, "", RegexOptions.Multiline);
      }

      File.WriteAllText(copyFileName, content);
      return copyFileName;
    }
  }
}
