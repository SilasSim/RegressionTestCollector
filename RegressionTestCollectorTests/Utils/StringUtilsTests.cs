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
    public void StringContainsAllStrings_StringContainsString_ReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less"));
      
    }

    [Test]
    public void StringContainsAllStrings_StringContainsMultipleString_ReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less", "more", "string"));

    }

    [Test]
    public void StringContainsAllStrings_StringDoesNotContainMultipleString_ReturnsFalse()
    {
      string suv = "A more or less long test string";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "less", "more", "DoesntContainThis"));

    }

    [Test]
    public void StringContainsAllStrings_StringDoesNotContainString_ReturnsFalse()
    {
      string suv = "A more or less long test string";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "TestString"));

    }

    [Test]
    public void StringContainsAllStrings_StringContainsString_IgnoresCapitilizationAndReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "less"));

    }

    [Test]
    public void StringContainsAllStrings_StringContainsMultipleString_IgnoresCapitilizationAndReturnsTrue()
    {
      string suv = "A more or less long test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "LESS", "mOrE", "sTRIng"));

    }

    #endregion

    #region CreateCommandString Tests

    [Test]
    public void CreateCommandString_WindowsAbsoluteWithExe_ReplacesRootDirWithAbsolutePath()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: true, isWindows: true);

      Assert.That(result, Is.EqualTo("" +
                                     "D:\\dev\\CANdelaStudio\\candelastudio\\builds\\windows-x86_64-vs-16-md-release\\deploy\\bin\\candelastudio-se.exe v23compat " +
                                     "-e dextImport -r D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
                                     "-i D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
                                     "-o D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_WindowsAbsoluteWithoutExe_ReplacesRootDirAndRemovesExe()
    {
      // Act
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: false, isWindows: true);

      // Assert
      Assert.That(result, Is.EqualTo(
        "v23compat -e dextImport -r " +
        "D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
        "-i D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
        "-o D:\\dev\\CANdelaStudio\\candelastudio\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_WindowsRelativeWithExe_KeepsOnlyExeName()
    {
      // Act
      var result =
        StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: true, isWindows: true);

      // Assert
      Assert.That(result, Is.EqualTo(
        "candelastudio-se.exe v23compat -e dextImport " +
        "-r ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
        "-i ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
        "-o ..\\..\\..\\..\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_WindowsRelativeWithoutExe_RemovesExeCompletely()
    {
      // Act
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: false, isWindows: true);

      // Assert
      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\ABS_ESP-Example.cdd " +
                                     "-i ..\\..\\..\\..\\CANdelaStudioSETest\\Source\\importDEXT.arxml " +
                                     "-o ..\\..\\..\\..\\CANdelaStudioSETest\\Check\\ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_LinuxAbsoluteWithExe_ConvertsToLinuxPath()
    {
      // Act
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: true, isWindows: false);

      // Assert
      Assert.That(result, Is.EqualTo("" +
                                     "/home/user/.vs/candelastudio/builds/linux-x86_64-clang-15-libstdc++11-release/deploy/bin/candelastudio-se " +
                                     "v23compat -e dextImport " +
                                     "-r /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/ABS_ESP-Example.cdd" +
                                     " -i /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o /home/user/.vs/candelastudio/CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_LinuxAbsoluteWithoutExe_ConvertsToLinuxPathAndRemovesExe()
    {
      // Act
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: true, isExeIncluded: false, isWindows: false);

      // Assert
      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i /home/user/.vs/candelastudio/CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o /home/user/.vs/candelastudio/CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_LinuxRelativeWithExe_ConvertsToLinuxAndKeepsExeName()
    {
      // Act
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: true, isWindows: false);

      // Assert
      Assert.That(result, Is.EqualTo("" +
                                     "candelastudio-se v23compat " +
                                     "-e dextImport " +
                                     "-r ../../../../CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i ../../../../CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o ../../../../CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_LinuxRelativeWithoutExe_RemovesExeCompletely()
    {
      var result = StringUtils.CreateCommandString(mSuvViewModel, isAbsPath: false, isExeIncluded: false, isWindows: false);

      Assert.That(result, Is.EqualTo("" +
                                     "v23compat -e dextImport " +
                                     "-r ../../../../CANdelaStudioSETest/Source/ABS_ESP-Example.cdd " +
                                     "-i ../../../../CANdelaStudioSETest/Source/importDEXT.arxml " +
                                     "-o ../../../../CANdelaStudioSETest/Check/ImportDextWithDeactivateServicesScenario_out.cdd -deact 1"));
    }

    [Test]
    public void CreateCommandString_LinuxConversion_ReplacesWindowsPlatformInAllPaths()
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
    public void GetArgumentFromString_FindsArgumentWithEquals()
    {
      string command = "myapp.exe -r input.txt -o=output.txt -v 1";

      var result = StringUtils.GetArgumentFromString(command, "-o");

      Assert.That(result, Is.EqualTo("output.txt"));
    }

    [Test]
    public void GetArgumentFromString_FindsArgumentWithSpace()
    {
      string command = "myapp.exe -r input.txt -o output.txt -v 1";

      var result = StringUtils.GetArgumentFromString(command, "-r");

      Assert.That(result, Is.EqualTo("input.txt"));
    }

    [Test]
    public void GetArgumentFromString_ArgumentNotFound_ReturnsEmpty()
    {
      string command = "myapp.exe -r input.txt -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-x");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetArgumentFromString_QuotedArgument_RemovesQuotes()
    {
      string command = "myapp.exe -f \"file with spaces.txt\" -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-f");

      Assert.That(result, Is.EqualTo("file with spaces.txt"));
    }

    [Test]
    public void GetArgumentFromString_SingleQuotedArgument_RemovesQuotes()
    {
      string command = "myapp.exe -f 'file with spaces.txt' -o output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-f");

      Assert.That(result, Is.EqualTo("file with spaces.txt"));
    }

    [Test]
    public void GetArgumentFromString_ArgumentWithEqualsAtEnd_ReturnsCorrectValue()
    {
      string command = "myapp.exe -r input.txt -version=1.2.3";

      var result = StringUtils.GetArgumentFromString(command, "-version");

      Assert.That(result, Is.EqualTo("1.2.3"));
    }

    [Test]
    public void GetArgumentFromString_EmptyValue_ReturnsEmpty()
    {
      string command = "myapp.exe -empty \"\" -other value";

      var result = StringUtils.GetArgumentFromString(command, "-empty");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetArgumentFromString_ValueWithSpecialCharacters_ReturnsCorrectValue()
    {
      string command = "myapp.exe -path \"C:\\Program Files\\MyApp\\file.txt\" -config settings.xml";

      var result = StringUtils.GetArgumentFromString(command, "-path");

      Assert.That(result, Is.EqualTo("C:\\Program Files\\MyApp\\file.txt"));
    }


    [Test]
    public void GetArgumentFromString_CaseSensitiveArgumentName_DoesNotMatch()
    {
      string command = "myapp.exe -Output file.txt -other value";

      var result = StringUtils.GetArgumentFromString(command, "-output");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetArgumentFromString_ArgumentWithDashes_ReturnsCorrectValue()
    {
      string command = "myapp.exe --long-option value-with-dashes --other test";

      var result = StringUtils.GetArgumentFromString(command, "--long-option");

      Assert.That(result, Is.EqualTo("value-with-dashes"));
    }

    [Test]
    public void GetArgumentFromString_EmptyCommand_ReturnsEmpty()
    {
      string command = "";

      var result = StringUtils.GetArgumentFromString(command, "-any");

      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetArgumentFromString_NullCommand_ThrowsArgumentNullException()
    {
      string? command = null;

      Assert.Throws<ArgumentNullException>(() => StringUtils.GetArgumentFromString(command!, "-any"));
    }

    [Test]
    public void GetArgumentFromString_MultipleSpacesBetweenArguments_ReturnsCorrectValue()
    {
      string command = "myapp.exe   -r    input.txt   -o     output.txt";

      var result = StringUtils.GetArgumentFromString(command, "-r");

      Assert.That(result, Is.EqualTo("input.txt"));
    }

    #endregion

    #region StringContainsAllStrings Additional Tests

    [Test]
    public void StringContainsAllStrings_EmptySourceString_ReturnsFalse()
    {
      string suv = "";
      Assert.IsFalse(StringUtils.StringContainsAllStrings(suv, "test"));
    }

    [Test]
    public void StringContainsAllStrings_EmptySearchString_ReturnsTrue()
    {
      string suv = "A test string";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, ""));
    }

    [Test]
    public void StringContainsAllStrings_NullSearchString_ThrowsException()
    {
      string suv = "A test string";
      Assert.Throws<ArgumentNullException>(() => StringUtils.StringContainsAllStrings(suv, null!));
    }

    [Test]
    public void StringContainsAllStrings_SpecialCharacters_ReturnsTrue()
    {
      string suv = "File path: C:\\Program Files\\App-v1.2.3\\config.xml";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, "C:\\", "App-v1.2.3", ".xml"));
    }

    [Test]
    public void StringContainsAllStrings_WhitespaceOnly_ReturnsTrue()
    {
      string suv = "   ";
      Assert.IsTrue(StringUtils.StringContainsAllStrings(suv, " "));
    }

    #endregion

    #region CreateCommandString Edge Cases

    [Test]
    public void CreateCommandString_EmptyTestCommand_ReturnsProcessedEmptyString()
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
    public void CreateCommandString_QuotedExecutablePath_HandlesCorrectly()
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
    public void ParseSearchTerms_EmptyInput_ReturnsEmptyArray()
    {
      var result = StringUtils.ParseSearchTerms("");
      Assert.IsEmpty(result);
    }

    [Test]
    public void ParseSearchTerms_NullOrWhitespace_ReturnsEmptyArray()
    {
      var result = StringUtils.ParseSearchTerms("   ");
      Assert.IsEmpty(result);
    }

    [Test]
    public void ParseSearchTerms_SimpleTerms_ReturnsSplitTerms()
    {
      var result = StringUtils.ParseSearchTerms("apple banana cherry");
      Assert.That(new[] { "apple", "banana", "cherry" }, Is.EqualTo(result));
    }

    [Test]
    public void ParseSearchTerms_WithQuotes_ReturnsGroupedTerms()
    {
      var result = StringUtils.ParseSearchTerms("\"apple pie\" banana \"cherry tart\"");
      Assert.That(new[] { "apple pie", "banana", "cherry tart" }, Is.EqualTo(result));
    }

    [Test]
    public void ParseSearchTerms_WithSingleQuote_ReplacesQuoteAndSplits()
    {
      var result = StringUtils.ParseSearchTerms("\"apple pie banana cherry");
      Assert.That(new[] { "apple", "pie", "banana", "cherry" }, Is.EqualTo(result));
    }

    [Test]
    public void ParseSearchTerms_CacheHit_ReturnsCachedValue()
    {
      var cache = new Dictionary<string, string[]>
        {
            { "cached", new[] { "from", "cache" } }
        };

      var result = StringUtils.ParseSearchTerms("cached", cache);
      Assert.That(new[] { "from", "cache" }, Is.EqualTo(result));
    }

    [Test]
    public void ParseSearchTerms_CacheMiss_AddsToCache()
    {
      var cache = new Dictionary<string, string[]>();
      var input = "new term";

      var result = StringUtils.ParseSearchTerms(input, cache);
      Assert.That(new[] { "new", "term" }, Is.EqualTo(result));
      Assert.IsTrue(cache.ContainsKey(input));
    }

    [Test]
    public void ParseSearchTerms_CacheClear_WhenLimitReached()
    {
      var cache = new Dictionary<string, string[]>();
      for (int i = 0; i < 100; i++)
      {
        cache[$"key{i}"] = new[] { $"value{i}" };
      }

      var result = StringUtils.ParseSearchTerms("new input", cache, 100);
      Assert.That(new[] { "new", "input" }, Is.EqualTo(result));
      Assert.That(1, Is.EqualTo(cache.Count));
    }


    #endregion

  }
}
