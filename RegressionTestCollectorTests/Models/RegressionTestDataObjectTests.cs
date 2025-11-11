using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.Models
{
  [TestFixture]
  public class RegressionTestDataObjectTests
  {
    private RegressionTestDataObject mSut;

    [SetUp]
    public void Setup()
    {
      mSut = new RegressionTestDataObject(
        testName: "SampleTest",
        testGroup: "UnitTests",
        testCommand: "run-test.exe --input file.txt",
        folderPath: @"C:\Tests",
        rootDir: @"..\data",
        pythonScriptPath: @"scripts\test.py"
      );
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithAllParameters_SetsPropertiesCorrectly()
    {
      var sut = new RegressionTestDataObject(
        "TestName",
        "TestGroup", 
        "TestCommand",
        "FolderPath",
        "RootDir",
        "PythonPath"
      );

      Assert.That(sut.TestName, Is.EqualTo("TestName"));
      Assert.That(sut.TestGroup, Is.EqualTo("TestGroup"));
      Assert.That(sut.TestCommand, Is.EqualTo("TestCommand"));
      Assert.That(sut.FolderPath, Is.EqualTo("FolderPath"));
      Assert.That(sut.RootDir, Is.EqualTo("RootDir"));
      Assert.That(sut.PythonScriptPath, Is.EqualTo("PythonPath"));
    }

    [Test]
    public void Constructor_WithMinimalParameters_SetsDefaults()
    {
      var sut = new RegressionTestDataObject("TestName");

      Assert.That(sut.TestName, Is.EqualTo("TestName")); 
      Assert.That(sut.TestGroup, Is.EqualTo(String.Empty));
      Assert.That(sut.TestCommand, Is.EqualTo(String.Empty));
      Assert.That(sut.FolderPath, Is.EqualTo(String.Empty));
      Assert.That(sut.RootDir, Is.EqualTo(String.Empty));
      Assert.That(sut.PythonScriptPath, Is.EqualTo(String.Empty));
      Assert.That(sut.Scenario, Is.EqualTo(String.Empty));
      Assert.That(sut.InputFile, Is.EqualTo(String.Empty));
      Assert.That(sut.OutputFile, Is.EqualTo(String.Empty));
    }

    #endregion

    #region Contains Method Tests

    [Test]
    public void Contains_MatchingTestName_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_MatchingTestCommand_ReturnsTrue()
    {
      var result = mSut.ContainsAny("run-test");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_MatchingTestGroup_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Unit");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_CaseInsensitive_ReturnsTrue()
    {
      var result = mSut.ContainsAny("SAMPLE", "unit", "RUN-TEST");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_MultipleMatchingStrings_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "Tests");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_PartialMatch_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Test", "run");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_NonMatchingString_ReturnsFalse()
    {
      var result = mSut.ContainsAny("NotFound");

      Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_EmptyString_ReturnsTrue()
    {
      var result = mSut.ContainsAny("");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_EmptyArray_ReturnsFalse()
    {
      var result = mSut.ContainsAny();

      Assert.That(result, Is.False);
    }

    [Test]
    public void Contains_WithSpecialCharacters_HandlesCorrectly()
    {
      var testObj = new RegressionTestDataObject("Test@Name", "Group#1", "command.exe --flag=value");

      Assert.That(testObj.ContainsAny("@"), Is.True);
      Assert.That(testObj.ContainsAny("#1"), Is.True);
      Assert.That(testObj.ContainsAny("--flag"), Is.True);
    }

    [Test]
    public void Contains_WithWhitespace_HandlesCorrectly()
    {
      var testObj = new RegressionTestDataObject("Test Name", "Group A", "run test");

      Assert.That(testObj.ContainsAny("Test Name"), Is.True);
      Assert.That(testObj.ContainsAny("Group A"), Is.True);
      Assert.That(testObj.ContainsAny("run test"), Is.True);
    }

    [Test]
    public void Contains_WithNullProperties_HandlesGracefully()
    {
      var testObj = new RegressionTestDataObject("");
      testObj.TestName = null!;
      testObj.TestCommand = null!;
      testObj.TestGroup = null!;

      Assert.DoesNotThrow(() => testObj.ContainsAny("test"));
      Assert.That(testObj.ContainsAny("test"), Is.False);
    }

    [Test]
    public void Contains_MatchesAcrossMultipleProperties_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "run-test", "Unit");

      Assert.That(result, Is.True);
    }

    [Test]
    public void Contains_OneStringMustMatch_ReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "NonExistent");

      Assert.That(result, Is.True);
    }

    #endregion

    #region Property Tests

    [Test]
    public void Properties_CanBeSetAndRetrieved()
    {
      mSut.Scenario = "TestScenario";
      mSut.InputFile = "input.txt";
      mSut.OutputFile = "output.txt";

      Assert.That(mSut.Scenario, Is.EqualTo("TestScenario"));
      Assert.That(mSut.InputFile, Is.EqualTo("input.txt"));
      Assert.That(mSut.OutputFile, Is.EqualTo("output.txt"));
    }

    [Test]
    public void Properties_DefaultValues_AreCorrect()
    {
      var sut = new RegressionTestDataObject("Test");

      Assert.That(sut.Scenario, Is.EqualTo(String.Empty));
      Assert.That(sut.InputFile, Is.EqualTo(String.Empty));
      Assert.That(sut.OutputFile, Is.EqualTo(String.Empty));
    }

    #endregion
  }
}