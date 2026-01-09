using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using NUnit.Framework;
using RegressionTestCollector.CollectingStrategies;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.CollectingStrategies
{
  [TestFixture]
  public class CSharpCollectingStrategyTests
  {
    private CSharpCollectingStrategy mSut;
    private bool mProgressChangedFired;

    [SetUp]
    public void Setup()
    {
      mProgressChangedFired = false;
      mSut = new CSharpCollectingStrategy((sender, e) => mProgressChangedFired = true);
    }

    [Test]
    public void WhenPropertiesAreSet_PropertiesCanBeSetAndRetrieved()
    {
      mSut.BatFilePattern = "TEST*.bat";
      mSut.PythonScriptPattern = "TEST(.py";
      mSut.PythonCommandVersion = "python2";

      Assert.That(mSut.BatFilePattern, Is.EqualTo("TEST*.bat"));
      Assert.That(mSut.PythonScriptPattern, Is.EqualTo("TEST(.py"));
      Assert.That(mSut.PythonCommandVersion, Is.EqualTo("python2"));
    }

    [Test]
    public void WhenConstructorIsCalled_ThenDefaultValuesAreSet()
    {
      Assert.That(mSut.BatFilePattern, Is.EqualTo("RegressionTest*.bat"));
      Assert.That(mSut.PythonScriptPattern, Is.EqualTo("RegTest*.py"));
      Assert.That(mSut.PythonCommandVersion, Is.EqualTo("python"));
    }

    #region FormatOutputString Tests

    [Test]
    public void GivenValidArrayString_WhenFormatOutputStringIsCalled_ThenReturnsFormatedString()
    {
      var input = "['/path/to/file', '--debug', 'value with spaces']";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Is.EqualTo("-path\\to\\file --debug \"value with spaces\""));
    }

    [Test]
    public void GivenEscapedWindowsPath_WhenFormatOutputStringIsCalled_ThenReturnsUnescapedPath()
    {
      var input = "['C:\\\\Program Files\\\\app.exe', '--arg']";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Does.Contain("C:\\Program Files\\app.exe"));
    }

    [Test]
    public void GivenLeadingBackslashes_WhenFormatOutputStringIsCalled_ThenReplacesBlackslashesWithHyphens()
    {
      var input = "['\\debug', '\\verbose']";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Does.Contain("-debug"));
      Assert.That(result, Does.Contain("-verbose"));
    }

    [Test]
    public void GivenSpacesInInput_WhenFormatOutputStringIsCalled_ThenReturnsStringWithArgumentsInQuotation()
    {
      var input = "['file name with spaces.txt', 'no-spaces']";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Does.Contain("\"file name with spaces.txt\""));
      Assert.That(result, Does.Not.Contain("\"no-spaces\""));
    }

    [Test]
    public void GivenInvalidFormat_WhenFormatOutputStringIsCalled_ThenReturnsEmptyString()
    {
      var input = "not an array format";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenEmptyArray_WhenFormatOutputStringIsCalled_ThenReturnsEmptyString()
    {
      var input = "[]";

      var result = CSharpCollectingStrategy.FormatOutputString(input);

      Assert.That(result, Is.EqualTo(""));
    }

    #endregion

    #region CheckForMissingBatArguments

    [Test]
    public void GivenAllRequiredArguments_WhenCheckForMissingBatArgumentsIsCalled_ReturnsEmptyArray()
    {
      var batArgs = new Dictionary<string, string>
      {
        ["def"] = "test.def",
        ["exeTarget"] = "app.exe",
        ["rootDir"] = "C:\\Root",
        ["useConfig"] = "config.xml"
      };

      var missing = CSharpCollectingStrategy.CheckForMissingBatArguments(batArgs);

      Assert.That(missing, Is.Empty);
    }

    [Test]
    public void GivenNoDefArgument_WhenCheckForMissingBatArgumentsIsCalled_ThenReturnsArrayWithMissingArgumentNameDef()
    {
      var batArgs = new Dictionary<string, string>
      {
        ["exeTarget"] = "app.exe",
        ["rootDir"] = "C:\\Root",
        ["useConfig"] = "config.xml"
      };

      var missing = CSharpCollectingStrategy.CheckForMissingBatArguments(batArgs);

      Assert.That(missing, Contains.Item("def"));
      Assert.That(missing.Length, Is.EqualTo(1));
    }

    [Test]
    public void GivenMultipleMissingArguments_WhenCheckForMissingBatArgumentsIsCalled_ThenReturnsArrayWithAllMissingArguments()
    {
      var batArgs = new Dictionary<string, string>
      {
        ["def"] = "test.def"
      };

      var missing = CSharpCollectingStrategy.CheckForMissingBatArguments(batArgs);

      Assert.That(missing, Contains.Item("exeTarget"));
      Assert.That(missing, Contains.Item("rootDir"));
      Assert.That(missing, Contains.Item("useConfig"));
      Assert.That(missing, Has.Length.EqualTo(3));
    }

    [Test]
    public void GivenEmptyDictionary_WhenCheckForMissingBatArgumentsIsCalled_ReturnsArrayWithAllRequiredArguments()
    {
      var batArgs = new Dictionary<string, string>();

      var missing = CSharpCollectingStrategy.CheckForMissingBatArguments(batArgs);

      Assert.That(missing.Length, Is.EqualTo(4));
      Assert.That(missing, Contains.Item("def"));
      Assert.That(missing, Contains.Item("exeTarget"));
      Assert.That(missing, Contains.Item("rootDir"));
      Assert.That(missing, Contains.Item("useConfig"));
    }

    #endregion

  }

}