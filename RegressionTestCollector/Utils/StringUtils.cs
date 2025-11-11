using System.Diagnostics;
using System.IO;
using RegressionTestCollector.ViewModel;
using System.Text.RegularExpressions;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.Utils
{
  public static class StringUtils
  {
    public static bool StringContainsAllStrings(string s, params string[] paramStrings)
    {
      return paramStrings.All(str => s.Contains(str, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetArgumentFromString(string s, string arg)
    {
      var match = Regex.Match(s, $@"{Regex.Escape(arg)}(?:=|\s+)([""']?)(.*?)\1(?=\s|$)");
      return match.Success ? match.Groups[2].Value : string.Empty;
    }


    public static string CreateCommandString(RegressionTestViewModel obj, bool isAbsPath, bool isExeIncluded,
      bool isWindows,
      bool isDebugSet = false, bool isVerboseSet = false)
    {
      var commandString = obj.RegressionTestData.TestCommand;
      var absoluteRootDir = Path.GetFullPath(obj.RegressionTestData.RootDir, obj.RegressionTestData.FolderPath);

      if (isAbsPath)
      {

        if (!isWindows)
        {
          string lastSegment = Path.GetFileName(absoluteRootDir.TrimEnd(Path.DirectorySeparatorChar));
          absoluteRootDir = "/home/user/.vs/" + lastSegment;
        }

        commandString = commandString.Replace(obj.RegressionTestData.RootDir, absoluteRootDir);
      }
      else
      {
        var arguments = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var exeToken = arguments.Length > 0 ? arguments[0].Trim('"') : string.Empty;
        commandString = commandString.Replace(exeToken, Path.GetFileName(exeToken));


        var exeAbs = Path.GetFullPath(exeToken, obj.RegressionTestData.FolderPath);
        var exeDir = Path.GetDirectoryName(exeAbs);

        if (string.IsNullOrEmpty(exeDir))
        {
          return "";
        }


        var relativeRootDir = Path.GetRelativePath(exeDir, absoluteRootDir);

        commandString = commandString.Replace(obj.RegressionTestData.RootDir, relativeRootDir);

        var rootDirSlash = obj.RegressionTestData.RootDir.Replace("\\", "/");
        var relRootSlash = relativeRootDir.Replace("\\", "/");
        commandString = commandString.Replace(rootDirSlash, relRootSlash);
      }

      if (!isExeIncluded)
      {
        var arguments = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var argumentsWithoutExe = arguments.Skip(1);
        commandString = String.Join(' ', argumentsWithoutExe);
      }

      if (!isWindows)
      {
        commandString = commandString.Replace("windows-x86_64-vs-16-md", "linux-x86_64-clang-15-libstdc++11");
        commandString = commandString.Replace(@"\", @"/");
        commandString = commandString.Replace(".exe", "");
      }

      if (isDebugSet)
      {
        var index = 0;
        if (isExeIncluded)
        {
          index = commandString.IndexOf(' ') + 1;
        }

        commandString = commandString.Insert(index, "-d ");
      }

      if (isVerboseSet)
      {
        var index = 0;
        if (isExeIncluded)
        {
          index = commandString.IndexOf(' ') + 1;
        }

        commandString = commandString.Insert(index, "-v ");
      }

      return commandString;
    }

    public static string[] ParseSearchTerms(string input, Dictionary<string, string[]>? cache = null, int clearCount = 100)
    {


      if (cache is not null && cache.ContainsKey(input))
      {
        return cache[input];
      }

      if (string.IsNullOrWhiteSpace(input))
      {
        return Array.Empty<string>();
      }

      if (cache is not null && cache.Count == clearCount)
      {
        cache.Clear();
      }

      string[] values;

      if (input.Count(c => c == '\"') > 1)
      {
        var matches = Regex.Matches(input, "\"([^\"]+)\"|(\\S+)", RegexOptions.IgnoreCase);
        var terms = new List<string>();

        foreach (Match match in matches)
        {
          var term = (match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);

          if (!String.IsNullOrWhiteSpace(term) && term != "\"")
          {
            terms.Add(term);
          }
        }

        values = terms.ToArray();
      }
      else
      {
        values = input.Replace('\"', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      }

      if (cache is not null)
      {
        cache[input] = values;
      }

      return values;
    }
  }
}
