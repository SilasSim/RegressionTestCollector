using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Models;
using RegressionTestCollector.Utils;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollectorTests.Utils
{
  [TestFixture]
  public class StringUtilsTests
  {
    private RegressionTestViewModel mSuvViewModel = new(new RegressionTestDataObject("Test")
    {
      TestCommand = "..\\..\\builds\\windows-x86_64-vs-16-md-release\\deploy\\bin\\candelastudio-se.exe v23compat -e dextImport " +
                    "-r ..\\..\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
                    "-i ..\\..\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
                    "-o ..\\..\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1",
      FolderPath = "D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\DeprecatedCLIs",
      RootDir = "..\\.."
    });

    #region StringContainsAllStrings Tests

    [Test]
    public void GivenSourceContainsSingleSearchString_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less"));
      
    }

    [Test]
    public void GivenSourceContainsAllSearchStrings_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less", "more", "string"));

    }

    [Test]
    public void GivenSourceMissingOneSearchString_WhenStringContainsAllStringsIsCalled_ThenReturnsFalse()
    {
      string suv = "A more or less long test string";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "less", "more", "DoesntContainThis"));

    }

    [Test]
    public void GivenSourceDoesNotContainSearchString_WhenStringContainsAllStringsIsCalled_ThenReturnsFalse()
    {
      string suv = "A more or less long test string";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "TestString"));

    }

    [Test]
    public void GivenSourceContainsSearchStringWithDifferentCase_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less"));

    }

    [Test]
    public void GivenSourceContainsMultipleSearchStringsWithDifferentCase_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "LESS", "mOrE", "sTRIng"));

    }

    #endregion

    #region CreateCommandString Tests

    [Test]
    public void GivenWindowsAbsolutePathAndExeIncluded_WhenCreateCommandStringIsCalled_ThenReplacesRootDirWithAbsolutePath()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: true, isWindows: true);

      Assert.That(result, Is.EqualTo("" +
                                     "D:\\dev\\CANdelaStudio\\candelastudio\\builds\\windows-x86_64-vs-16-md-release\\deploy\\bin\\candelastudio-se.exe v23compat " +
                                     "-e dextImport -r D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
                                     "-i D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
                                     "-o D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenWindowsAbsolutePathAndExeExcluded_WhenCreateCommandStringIsCalled_ThenReplacesRootDirAndRemovesExe()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: false, isWindows: true);

      Assert.That(result, Is.EqualTo(
        "v23compat -e dextImport -r " +
        "D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
        "-i D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
        "-o D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenWindowsRelativePathAndExeIncluded_WhenCreateCommandStringIsCalled_ThenStripsDirectoryPathFromExe()
    {
      var result =
        StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: true, isWindows: true);

      Assert.That(result, Is.EqualTo(
        "candelastudio-se.exe v23compat -e dextImport " +
        "-r ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
        "-i ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
        "-o ..\\..\\..\\..\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenWindowsRelativePathAndExeExcluded_WhenCreateCommandStringIsCalled_ThenRemovesExeCompletely()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: false, isWindows: true);

      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
                                     "-i ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
                                     "-o ..\\..\\..\\..\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenLinuxAbsolutePathAndExeIncluded_WhenCreateCommandStringIsCalled_ThenConvertsToLinuxPath()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: true, isWindows: false);

      Assert.That(result, Is.EqualTo("" +
                                     "/home/user/.vs/candelastudio/builds/linux-x86_64-clang-15-libstdc++11-release/deploy/bin/candelastudio-se " +
                                     "v23compat -e dextImport " +
                                     "-r /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/ABS_ESP-Example.cdd" +
                                     " -i /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o /home/user/.vs/candelastudio/CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenLinuxAbsolutePathAndExeExcluded_WhenCreateCommandStringIsCalled_ThenConvertsToLinuxPathAndRemovesExe()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: false, isWindows: false);

      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o /home/user/.vs/candelastudio/CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenLinuxRelativePathAndExeIncluded_WhenCreateCommandStringIsCalled_ThenConvertsToLinuxAndKeepsExeName()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: true, isWindows: false);

      Assert.That(result, Is.EqualTo("" +
                                     "candelastudio-se v23compat " +
                                     "-e dextImport " +
                                     "-r ../../../../CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i ../../../../CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o ../../../../CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenLinuxRelativePathAndExeExcluded_WhenCreateCommandStringIsCalled_ThenRemovesExeCompletely()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: false, isWindows: false);

      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r ../../../../CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i ../../../../CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o ../../../../CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void GivenLinux_WhenCreateCommandStringIsCalled_ThenReplacesWindowsPlatformInAllPaths()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: true, isWindows: false);

      Assert.That(result, Does.Not.Contain("windows-x86_64-vs-16-md"));
      Assert.That(result, Does.Contain("linux-x86_64-clang-15-libstdc++11"));
      Assert.That(result, Does.Not.Contain("\\"));
      Assert.That(result, Does.Contain("/"));
    }

    #endregion

    #region GetArgumentFromString Tests

    [Test]
    public void GivenCommandWithArgumentUsingEquals_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe -r input.txt -o=output.txt -v 1";

      var result = StringUtils.GetArgumentFromString(command, "-o");

      Assert.That(result, Is.EqualTo("output.txt"));
    }

    [Test]
    public void GivenCommandWithArgumentUsingSpace_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe -r input.txt -o output.txt -v 1";

      var result = StringUtils.GetArgumentFromString(command, "-r");

      Assert.That(result, Is.EqualTo("input.txt"));
    }

    [Test]
    public void GivenCommandWithoutRequestedArgument_WhenGetArgumentFromStringIsCalled_ThenReturnsEmpty()
    {
      string command = "myapp.exe -r input.txt -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-x");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenCommandWithDoubleQuotedArgument_WhenGetArgumentFromStringIsCalled_ThenRemovesQuotes()
    {
      string command = "myapp.exe -f \"file with spaces.txt\" -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-f");

      Assert.That(result, Is.EqualTo("file with spaces.txt"));
    }

    [Test]
    public void GivenCommandWithSingleQuotedArgument_WhenGetArgumentFromStringIsCalled_ThenRemovesQuotes()
    {
      string command = "myapp.exe -f 'file with spaces.txt' -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-f");

      Assert.That(result, Is.EqualTo("file with spaces.txt"));
    }

    [Test]
    public void GivenCommandWithArgumentEqualsAtEnd_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe -r input.txt -version=1.2.3";

      var result = StringUtils.GetArgumentFromString(command, "-version");

      Assert.That(result, Is.EqualTo("1.2.3"));
    }

    [Test]
    public void GivenCommandWithEmptyArgumentValue_WhenGetArgumentFromStringIsCalled_ThenReturnsEmpty()
    {
      string command = "myapp.exe -empty \"\" -other value";

      var result = StringUtils.GetArgumentFromString(command, "-empty");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenCommandWithArgumentContainingSpecialCharacters_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe -path \"C:\\Program Files\\MyApp\\file.txt\" -config settings.xml";

      var result = StringUtils.GetArgumentFromString(command, "-path");

      Assert.That(result, Is.EqualTo("C:\\Program Files\\MyApp\\file.txt"));
    }


    [Test]
    public void GivenCommandWithDifferentCaseArgument_WhenGetArgumentFromStringIsCalled_ThenDoesNotMatch()
    {
      string command = "myapp.exe -Output file.txt -other value";

      var result = StringUtils.GetArgumentFromString(command, "-output");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenCommandWithArgumentContainingDashes_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe --long-option value-with-dashes --other test";

      var result = StringUtils.GetArgumentFromString(command, "--long-option");

      Assert.That(result, Is.EqualTo("value-with-dashes"));
    }

    [Test]
    public void GivenEmptyCommand_WhenGetArgumentFromStringIsCalled_ThenReturnsEmpty()
    {
      string command = "";

      var result = StringUtils.GetArgumentFromString(command, "-any");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GivenNullCommand_WhenGetArgumentFromStringIsCalled_ThenThrowsArgumentNullException()
    {
      string? command = null;

      Assert.Throws<ArgumentNullException>(() => StringUtils.GetArgumentFromString(command!, "-any"));
    }

    [Test]
    public void GivenCommandWithMultipleSpaces_WhenGetArgumentFromStringIsCalled_ThenReturnsCorrectValue()
    {
      string command = "myapp.exe   -r    input.txt   -o     output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-r");

      Assert.That(result, Is.EqualTo("input.txt"));
    }

    #endregion

    #region StringContainsAllStrings Additional Tests

    [Test]
    public void GivenEmptySource_WhenStringContainsAllStringsIsCalled_ThenReturnsFalse()
    {
      string suv = "";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "test"));
    }

    [Test]
    public void GivenNonEmptySourceAndEmptySearch_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "A test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, ""));
    }

    [Test]
    public void GivenNullSearchString_WhenStringContainsAllStringsIsCalled_ThenThrowsArgumentNullException()
    {
      string suv = "A test string";
      Assert.Throws<ArgumentNullException>(() => StringUtils.StringContainsAllStrings(suv, null!));
    }

    [Test]
    public void GivenSourceWithSpecialCharacters_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "File path: C:\\Program Files\\App-v1.2.3\\config.xml";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "C:\\", "App-v1.2.3", ".xml"));
    }

    [Test]
    public void GivenWhitespaceSource_WhenStringContainsAllStringsIsCalled_ThenReturnsTrue()
    {
      string suv = "   ";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, " "));
    }

    #endregion

    #region CreateCommandString Edge Cases

    [Test]
    public void GivenEmptyTestCommand_WhenCreateCommandStringIsCalled_ThenReturnsEmptyString()
    {
      var testViewModel = new RegressionTestViewModel(new RegressionTestDataObject("Test")
      {
        TestCommand = "",
        FolderPath = "D:\\dev\\test",
        RootDir = "..\\.."
      });

      var result = StringUtils.CreateCommandString(testViewModel, isAbsPath: true, isExeIncluded: true, isWindows: true);

      Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GivenQuotedExecutablePath_WhenCreateCommandStringIsCalled_ThenStripsFoldersAndKeepsQuotes()
    {
      var testViewModel = new RegressionTestViewModel(new RegressionTestDataObject("Test")
      {
        TestCommand = "\"..\\..\\builds\\app with spaces.exe\" -arg value",
        FolderPath = "D:\\dev\\test\\folder",
        RootDir = "..\\.."
      });

      var result = StringUtils.CreateCommandString(testViewModel, isAbsPath: false, isExeIncluded: true, isWindows: true);

      Assert.That(result, Does.StartWith("\"app with spaces.exe\""));
    }


    #endregion

    #region ParseSearchTerms Tests


    [Test]
    public void GivenEmptyInput_WhenParseSearchTermsIsCalled_ThenReturnsEmptyArray()
    {
      var result = StringUtils.ParseSearchTerms("");
      Assert.IsEmpty(result);
    }

    [Test]
    public void GivenNullOrWhitespaceInput_WhenParseSearchTermsIsCalled_ThenReturnsEmptyArray()
    {
      var result = StringUtils.ParseSearchTerms("   ");
      Assert.IsEmpty(result);
    }

    [Test]
    public void GivenSimpleTerms_WhenParseSearchTermsIsCalled_ThenReturnsSplitTerms()
    {
      var result = StringUtils.ParseSearchTerms("a b c");
      Assert.That(new[] { "a", "b", "c" }, Is.EqualTo(result));
    }

    [Test]
    public void GivenQuotedPhrases_WhenParseSearchTermsIsCalled_ThenReturnsGroupedTerms()
    {
      var result = StringUtils.ParseSearchTerms("\"a b\" c \"d e\"");
      Assert.That(new[] { "a b", "c", "d e" }, Is.EqualTo(result));
    }

    [Test]
    public void GivenUnclosedQuote_WhenParseSearchTermsIsCalled_ThenReplacesQuoteAndSplits()
    {
      var result = StringUtils.ParseSearchTerms("\"a b c d");
      Assert.That(new[] { "a", "b", "c", "d" }, Is.EqualTo(result));
    }

    [Test]
    public void GivenCacheWithHit_WhenParseSearchTermsIsCalled_ThenReturnsCachedValue()
    {
      var cache = new Dictionary<string, string[]>
        {
            { "cached", new[] { "from", "cache" } }
        };

      var result = StringUtils.ParseSearchTerms("cached", cache);
      Assert.That(new[] { "from", "cache" }, Is.EqualTo(result));
    }

    [Test]
    public void GivenEmptyCacheAndNewTerms_WhenParseSearchTermsIsCalled_ThenAddsToCache()
    {
      var cache = new Dictionary<string, string[]>();
      var input = "new term";

      var result = StringUtils.ParseSearchTerms(input, cache);
      Assert.That(new[] { "new", "term" }, Is.EqualTo(result));
      Assert.IsTrue(cache.ContainsKey(input));
    }

    [Test]
    public void GivenCacheAtLimit_WhenParseSearchTermsIsCalled_ThenClearsCacheAndStoresNewEntry()
    {
      var cache = new Dictionary<string, string[]>();
      for (int i = 0; i < 100; i++)
      {
        cache[$"key{i}"] = new[] { $"value{i}" };
      }

      var result = StringUtils.ParseSearchTerms("new input", cache, 100);
      Assert.That(result, Is.EqualTo(new[] { "new", "input" }));
      Assert.That(cache.Count, Is.EqualTo(1));
    }


    #endregion

  }
}
