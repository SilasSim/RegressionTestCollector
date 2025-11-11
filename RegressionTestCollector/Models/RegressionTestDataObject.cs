using System.Dynamic;

namespace RegressionTestCollector.Models
{
  public class RegressionTestDataObject
  {

    public string TestName { get; set; }

    public string TestCommand { get; set; }

    public string TestGroup { get; set; }
    public string FolderPath { get; set; }
    public string RootDir { get; set; }

    public string PythonScriptPath { get; set; }

    public string Scenario { get; set; } = String.Empty;

    public string InputFile { get; set; } = String.Empty;
    public string OutputFile { get; set; } = String.Empty;

    public RegressionTestDataObject(string testName, string testGroup = "", 
      string testCommand = "", string folderPath = "", string rootDir = "", 
      string pythonScriptPath = "")
    {
      TestName = testName;
      TestCommand = testCommand;
      TestGroup = testGroup;
      FolderPath = folderPath;
      RootDir = rootDir;
      PythonScriptPath = pythonScriptPath;
    }

    public bool ContainsAny(params string[] strings)
    {
      return strings.Any(s =>
        (TestName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (TestCommand?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
        (TestGroup?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
    }

  }
}
