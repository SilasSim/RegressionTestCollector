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
    public void WhenConstructorIsCalled_ThenSetsPropertiesCorrectly()
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
    public void GivenMinimalParameters_WhenConstructorIsCalled_ThenSetsDefaultValues()
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
    public void GivenStringWithMatchingValueInTestName_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenStringWithMatchingValueInTestCommand_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("run-test");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenStringWithMatchingValueInTestGroup_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Unit");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenStringsWithCaseInsensitiveMatchingValuesInRelevantProperties_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("SAMPLE", "unit", "RUN-TEST");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenMultipleStringsWithMatchingValuesInRelevantProperties_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "Tests");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenStringWithPartialMatchingValueInRelevantProperties_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Test", "run");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenStringWithNoMatchingValueInRelevantProperties_WhenContainsAnyIsCalled_ThenReturnsFalse()
    {
      var result = mSut.ContainsAny("NotFound");

      Assert.That(result, Is.False);
    }

    [Test]
    public void GivenEmptyString_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenEmptyArray_WhenContainsAnyIsCalled_ThenReturnsFalse()
    {
      var result = mSut.ContainsAny([]);

      Assert.That(result, Is.False);
    }

    [Test]
    public void GivenRelevantPropertiesContainSpecialCharacters_WhenContainsAnyIsCalled_ThenReturnsTrueForPartialMatches()
    {
      var testObj = new RegressionTestDataObject("Test@Name", "Group#1", "command.exe --flag=value");

      Assert.That(testObj.ContainsAny("@"), Is.True);
      Assert.That(testObj.ContainsAny("#1"), Is.True);
      Assert.That(testObj.ContainsAny("--flag"), Is.True);
    }

    [Test]
    public void GivenRelevantPropertiesAreNull_WhenContainsAnyIsCalled_ThenDoesNotThrowAndReturnsFalse()
    {
      var testObj = new RegressionTestDataObject("");
      testObj.TestName = null!;
      testObj.TestCommand = null!;
      testObj.TestGroup = null!;

      Assert.DoesNotThrow(() => testObj.ContainsAny("test"));
      Assert.That(testObj.ContainsAny("test"), Is.False);
    }

    [Test]
    public void GivenMultipleStringsMatchingAcrossMultipleRelevantProperties_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "run-test", "Unit");

      Assert.That(result, Is.True);
    }

    [Test]
    public void GivenMultipleStringsMatchingOneRelevantProperty_WhenContainsAnyIsCalled_ThenReturnsTrue()
    {
      var result = mSut.ContainsAny("Sample", "NonExistent");

      Assert.That(result, Is.True);
    }

    #endregion

    #region Property Tests

    [Test]
    public void WhenPropertiesAreSet_ThenPropertiesHaveCorrectValues()
    {
      mSut.Scenario = "TestScenario";
      mSut.InputFile = "input.txt";
      mSut.OutputFile = "output.txt";

      Assert.That(mSut.Scenario, Is.EqualTo("TestScenario"));
      Assert.That(mSut.InputFile, Is.EqualTo("input.txt"));
      Assert.That(mSut.OutputFile, Is.EqualTo("output.txt"));
    }

    #endregion
  }
}