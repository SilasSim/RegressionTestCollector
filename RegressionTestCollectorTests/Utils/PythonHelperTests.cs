using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.Models;
using RegressionTestCollector.Utils;

namespace RegressionTestCollectorTests.Utils
{
  [TestFixture]
  public class PythonHelperTests
  {
    private string mTestDirectory;
    private string mTestPythonFile;

    [SetUp]
    public void SetUp()
    {
      mTestDirectory = Path.Combine(Path.GetTempPath(), "PythonHelperTests", Guid.NewGuid().ToString());
      Directory.CreateDirectory(mTestDirectory);
      mTestPythonFile = Path.Combine(mTestDirectory, "test_script.py");
    }

    [TearDown]
    public void TearDown()
    {
      if (Directory.Exists(mTestDirectory))
      {
        Directory.Delete(mTestDirectory, true);
      }
    }

    #region ReadPythonOutputFile Tests

    [Test]
    public void GivenExistingOutputFile_WhenReadPythonOutputFileIsCalled_ThenReturnsFileContent()
    {
      string testContent = "Test output content\nSecond line";
      string testFile = Path.Combine(mTestDirectory, "output.txt");
      File.WriteAllText(testFile, testContent);

      var result = PythonHelper.ReadPythonOutputFile(testFile);

      Assert.That(result, Is.EqualTo(testContent));
    }

    [Test]
    public void GivenEmptyOutputFile_WhenReadPythonOutputFileIsCalled_ThenReturnsEmptyString()
    {
      string testFile = Path.Combine(mTestDirectory, "empty.txt");
      File.WriteAllText(testFile, "");

      var result = PythonHelper.ReadPythonOutputFile(testFile);

      Assert.That(result, Is.EqualTo(""));
    }

    #endregion

    #region CreateAndTransformCopy Tests

    [Test]
    public void GivenExistingPythonFile_WhenCreateAndTransformCopyIsCalled_ThenCreatesTransformedCopy()
    {
      string originalContent = "import module\nrequired=True\nother content\nServerApi.StartServer()";
      File.WriteAllText(mTestPythonFile, originalContent);

      var result = PythonHelper.CreateAndTransformCopy(
        mTestPythonFile, 
        "replacement", 
        new[] { "required=True", "ServerApi.StartServer()" }, 
        new[] { "import module" });

      Assert.That(File.Exists(result), Is.True);
      var transformedContent = File.ReadAllText(result);
      Assert.That(transformedContent, Does.Contain("replacement"));
      Assert.That(transformedContent, Does.Not.Contain("required=True"));
      Assert.That(transformedContent, Does.Not.Contain("ServerApi.StartServer()"));
      Assert.That(transformedContent, Does.Not.Contain("import module"));
    }

    [Test]
    public void GivenNoExistentPythonFile_WhenCreateAndTransformCopyIsCalled_ThenReturnsEmptyString()
    {
      string nonExistentFile = Path.Combine(mTestDirectory, "nonexistent.py");

      var result = PythonHelper.CreateAndTransformCopy(
        nonExistentFile, 
        "replacement", 
        new[] { "test" }, 
        new[] { "pattern" });

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenEmptyReplacementArrays_WhenCreateAndTransformCopyIsCalled_ThenCopiesFileUnchanged()
    {
      string originalContent = "unchanged content\nwith multiple lines";
      File.WriteAllText(mTestPythonFile, originalContent);

      var result = PythonHelper.CreateAndTransformCopy(
        mTestPythonFile, 
        "replacement", 
        new string[0], 
        new string[0]);

      Assert.That(File.Exists(result), Is.True);
      var copiedContent = File.ReadAllText(result);
      Assert.That(copiedContent, Is.EqualTo(originalContent));
    }

    [Test]
    public void GivenRegexPatternsForRemoval_WhenCreateAndTransformCopyIsCalled_ThenRemovesMatchingLines()
    {
      string originalContent = "line1\n# comment line\nline3\n## another comment\nline5";
      File.WriteAllText(mTestPythonFile, originalContent);

      var result = PythonHelper.CreateAndTransformCopy(
        mTestPythonFile, 
        "", 
        new string[0], 
        new[] { @"^#.*$" }); // Remove comment lines

      var transformedContent = File.ReadAllText(result);
      Assert.That(transformedContent, Does.Contain("line1"));
      Assert.That(transformedContent, Does.Contain("line3"));
      Assert.That(transformedContent, Does.Contain("line5"));
      Assert.That(transformedContent, Does.Not.Contain("# comment line"));
      Assert.That(transformedContent, Does.Not.Contain("## another comment"));
    }

    [Test]
    public void GivenMultipleReplacementTerms_WhenCreateAndTransformCopyIsCalled_ThenReplacesAllOccurrences()
    {
      string originalContent = "first string\nsecond string\nthird content";
      File.WriteAllText(mTestPythonFile, originalContent);

      var result = PythonHelper.CreateAndTransformCopy(
        mTestPythonFile, 
        "REPLACED", 
        new[] { "first", "second" }, 
        new string[0]);

      var transformedContent = File.ReadAllText(result);
      Assert.That(transformedContent, Is.EqualTo("REPLACED string\nREPLACED string\nthird content"));
    }

    #endregion

    #region CreateDebugCopy Tests

    [Test]
    public void GivenPythonScriptWithRequiredTrue_WhenCreateDebugCopyIsCalled_ThenReplacesWithRequiredFalse()
    {
      string pythonContent = "parser.add_argument('--input', required=True)\nother line";
      File.WriteAllText(mTestPythonFile, pythonContent);

      var testObject = new RegressionTestDataObject("Test")
      {
        PythonScriptPath = mTestPythonFile,
        FolderPath = mTestDirectory,
        RootDir = "..",
        InputFile = "input.txt",
        OutputFile = "output.txt", 
        Scenario = "test_scenario"
      };

      var result = PythonHelper.CreateDebugCopy(testObject);

      Assert.That(File.Exists(result), Is.True);
      var debugContent = File.ReadAllText(result);
      Assert.That(debugContent, Does.Contain("required=False"));
      Assert.That(debugContent, Does.Not.Contain("required=True"));
    }

    [Test]
    public void GivenPythonScriptWithServerStartCall_WhenCreateDebugCopyIsCalled_ThenCommentsOutServerStartLine()
    {
      string pythonContent = "    ServerApi.StartServer(config)\n    other_line()";
      File.WriteAllText(mTestPythonFile, pythonContent);

      var testObject = new RegressionTestDataObject("Test")
      {
        PythonScriptPath = mTestPythonFile,
        FolderPath = mTestDirectory,
        RootDir = "..",
        InputFile = "input.txt",
        OutputFile = "output.txt",
        Scenario = "test_scenario"
      };

      var result = PythonHelper.CreateDebugCopy(testObject);

      var debugContent = File.ReadAllText(result);
      Assert.That(debugContent, Does.Contain("# ServerApi.StartServer"));
      Assert.That(debugContent, Does.Not.Contain("    ServerApi.StartServer(config)"));
      Assert.That(debugContent, Does.Contain("    # ServerApi.StartServer(config)"));
    }

    [Test]
    public void GivenPythonScriptWithWaitForServerCall_WhenCreateDebugCopyIsCalled_ThenCommentsOutWaitForServerToStart()
    {
      string pythonContent = "some_function()\nwaitForServerToStart()\nother_code()";
      File.WriteAllText(mTestPythonFile, pythonContent);

      var testObject = new RegressionTestDataObject("Test")
      {
        PythonScriptPath = mTestPythonFile,
        FolderPath = mTestDirectory,
        RootDir = "..",
        InputFile = "input.txt",
        OutputFile = "output.txt",
        Scenario = "test_scenario"
      };

      var result = PythonHelper.CreateDebugCopy(testObject);

      var debugContent = File.ReadAllText(result);
      Assert.That(debugContent, Does.Contain("# waitForServerToStart()"));
      Assert.That(debugContent.Replace("# waitForServerToStart()", ""), Does.Not.Contain("waitForServerToStart()"));
    }

    [Test]
    public void GivenPythonScriptWithParseKnownArgsStatement_WhenCreateDebugCopyIsCalled_ThenInjectsInputOutputAndScenarioArgsInCopy()
    {
      string pythonContent = "args = parser.parse_known_args()\nrest_of_code()";
      File.WriteAllText(mTestPythonFile, pythonContent);

      var testObject = new RegressionTestDataObject("Test")
      {
        PythonScriptPath = mTestPythonFile,
        FolderPath = Path.Combine("D:", "test", "folder"),
        RootDir = "..\\..",
        InputFile = "..\\..\\input\\file.txt",
        OutputFile = "..\\..\\output\\file.txt",
        Scenario = "test_scenario"
      };

      var result = PythonHelper.CreateDebugCopy(testObject);

      var debugContent = File.ReadAllText(result);
      Assert.That(debugContent, Does.Contain("args.i = "));
      Assert.That(debugContent, Does.Contain("args.o = "));
      Assert.That(debugContent, Does.Contain("args.scenario = \"test_scenario\""));
      Assert.That(debugContent, Does.Contain("input\\\\file.txt"));
      Assert.That(debugContent, Does.Contain("output\\\\file.txt"));
    }

    [Test]
    public void GivenPythonScript_WhenCreateDebugCopyIsCalled_ThenCreatesFileWithDebugRtcSuffix()
    {
      File.WriteAllText(mTestPythonFile, "dummy content");

      var testObject = new RegressionTestDataObject("Test")
      {
        PythonScriptPath = mTestPythonFile,
        FolderPath = mTestDirectory,
        RootDir = "..",
        InputFile = "input.txt",
        OutputFile = "output.txt",
        Scenario = "test_scenario"
      };

      var result = PythonHelper.CreateDebugCopy(testObject);

      Assert.That(result, Does.EndWith("_debugRTC.py"));
      Assert.That(result, Does.Not.EndWith("test_script.py"));
    }

    #endregion

    #region RunProcess Tests

    [Test]
    public void GivenSimpleEchoCommand_WhenRunProcessIsCalled_ThenReturnsStandardOutput()
    {
      var processInfo = new ProcessStartInfo()
      {
        FileName = "cmd.exe",
        Arguments = "/c echo Hello World",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      var result = PythonHelper.RunProcess(processInfo);

      Assert.That(result, Does.Contain("Hello World"));
    }

    [Test]
    public void GivenOutputCallback_WhenRunProcessIsCalled_ThenInvokesOutputCallback()
    {
      var outputReceived = new List<string>();
      var processInfo = new ProcessStartInfo()
      {
        FileName = "cmd.exe",
        Arguments = "/c echo Test Output",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      PythonHelper.RunProcess(processInfo, output => outputReceived.Add(output));

      Assert.That(outputReceived, Has.Some.Matches<string>(s => s.Contains("Test Output")));
    }

    [Test]
    public void GivenProcessWithErrorCallback_WhenRunProcessIsCalled_ThenInvokesErrorCallbackOnStderr()
    {
      var errorsReceived = new List<string>();
      var processInfo = new ProcessStartInfo()
      {
        FileName = "cmd.exe",
        Arguments = "/c echo Error Message >&2",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      PythonHelper.RunProcess(processInfo, null, error => errorsReceived.Add(error));

      Assert.That(errorsReceived, Has.Some.Matches<string>(s => s.Contains("Error Message")));
    }

    #endregion

  }
}
