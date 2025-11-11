using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.Models;
using RegressionTestCollector.Parser;

namespace RegressionTestCollectorTests.Parser
{
  [TestFixture]
  public class RegressionTestConfigParserTests
  {
    private RegressionTestConfigParser mSut;
    private string mTestRootDir;

    [SetUp]
    public void Setup()
    {
      mSut = new RegressionTestConfigParser();
      mTestRootDir = @"C:\TestRoot";
    }

    #region Basic Parsing Tests

    [Test]
    public void Parse_ValidXmlWithEqualityTests_ReturnsCorrectData()
    {
      var xmlData = CreateBasicXmlWithEqualityTest();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Kind, Is.EqualTo(RegressionTestKind.EqualityTest));
      Assert.That(result.RegressionTests[0].Name, Is.EqualTo("Test1"));
    }

    [Test]
    public void Parse_ValidXmlWithFailTests_ReturnsCorrectData()
    {
      var xmlData = CreateBasicXmlWithFailTest();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Kind, Is.EqualTo(RegressionTestKind.FailTest));
      Assert.That(result.RegressionTests[0].Name, Is.EqualTo("FailTest1"));
    }

    [Test]
    public void Parse_EmptyXml_ReturnsEmptyResult()
    {
      var xmlData = "<root></root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Is.Empty);
    }

    [Test]
    public void Parse_InvalidXml_ThrowsException()
    {
      var xmlData = "<invalid><unclosed>";

      Assert.Throws<System.Xml.XmlException>(() => mSut.Parse(xmlData, mTestRootDir));
    }

    #endregion

    #region INI File Handling Tests

    [Test]
    public void Parse_WithIniFiles_MapsIniFilesCorrectly()
    {
      var xmlData = CreateXmlWithIniFiles();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain(mTestRootDir));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain("configs"));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain("config1.ini"));
    }

    [Test]
    public void Parse_WithRootDirReplacement_ReplacesCorrectly()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>%ROOTDIR;/configs</SOURCEDIR>
            <INIFILE id=""ini1"">test.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>%ROOTDIR;/source</SOURCEDIR>
            <TARGETDIR>%ROOTDIR;/target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      var test = result.RegressionTests[0];
      
      // Check path components instead of exact format
      Assert.That(test.Inifile, Does.Contain(mTestRootDir));
      Assert.That(test.Inifile, Does.Contain("configs"));
      Assert.That(test.Inifile, Does.Contain("test.ini"));
      
      Assert.That(test.Sourcefile, Does.Contain(mTestRootDir));
      Assert.That(test.Sourcefile, Does.Contain("source"));
      Assert.That(test.Sourcefile, Does.Contain("input.txt"));
      
      Assert.That(test.Outfile, Does.Contain(mTestRootDir));
      Assert.That(test.Outfile, Does.Contain("target"));
      Assert.That(test.Outfile, Does.Contain("output.txt"));
    }

    [Test]
    public void Parse_WithoutIniFileId_ThrowsKeyNotFoundException()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""nonexistent"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      Assert.Throws<KeyNotFoundException>(() => mSut.Parse(xmlData, mTestRootDir));
    }

    #endregion

    #region Multiple Test Types Tests

    [Test]
    public void Parse_WithMultipleTestTypes_ParsesAllTypes()
    {
      var xmlData = CreateXmlWithMultipleTestTypes();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result.RegressionTests, Has.Count.EqualTo(2));
      
      var equalityTest = result.RegressionTests.FirstOrDefault(t => t.Kind == RegressionTestKind.EqualityTest);
      var failTest = result.RegressionTests.FirstOrDefault(t => t.Kind == RegressionTestKind.FailTest);
      
      Assert.That(equalityTest, Is.Not.Null);
      Assert.That(failTest, Is.Not.Null);
      Assert.That(equalityTest.Name, Is.EqualTo("EqualTest1"));
      Assert.That(failTest.Name, Is.EqualTo("FailTest1"));
    }

    [Test]
    public void Parse_WithMultipleTestsOfSameType_ParsesAll()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input1.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output1.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
            <EQUALITYTEST Name=""Test2"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input2.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output2.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result.RegressionTests, Has.Count.EqualTo(2));
      Assert.That(result.RegressionTests.All(t => t.Kind == RegressionTestKind.EqualityTest), Is.True);
    }

    #endregion

    #region Additional Parameters Tests

    [Test]
    public void Parse_WithAdditionalParameters_ParsesCorrectly()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
              <ADDITIONAL-PARAMETERS>
                <PARAMETER value=""verbose"">v</PARAMETER>
                <PARAMETER value=""debug"">d</PARAMETER>
              </ADDITIONAL-PARAMETERS>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      var test = result.RegressionTests[0];
      Assert.That(test.AdditionalParameter, Has.Count.EqualTo(2));
      Assert.That(test.AdditionalParameter["v"], Is.EqualTo("verbose"));
      Assert.That(test.AdditionalParameter["d"], Is.EqualTo("debug"));
    }

    [Test]
    public void Parse_WithEmptyAdditionalParameters_HandlesGracefully()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
              <ADDITIONAL-PARAMETERS>
              </ADDITIONAL-PARAMETERS>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      var test = result.RegressionTests[0];
      Assert.That(test.AdditionalParameter, Is.Empty);
    }

    #endregion

    #region Edge Cases Tests

    [Test]
    public void Parse_WithMissingTestName_SkipsTest()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result.RegressionTests, Is.Empty);
    }

    [Test]
    public void Parse_WithMissingSourceFiles_CreatesTestWithEmptySource()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      var test = result.RegressionTests[0];
      Assert.That(test.Sourcefile, Is.EqualTo(Path.Join("source", "")));
    }

    [Test]
    public void Parse_WithLogFile_ParsesLogFileCorrectly()
    {
      var xmlData = @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
              <LOGFILE>test.log</LOGFILE>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      var test = result.RegressionTests[0];
      Assert.That(test.Logfile, Is.EqualTo("test.log"));
    }

    #endregion

    #region RegressionTestConfiguration Command String Tests

    [Test]
    public void GetCommandStringForPythonScript_WithAllParameters_GeneratesCorrectString()
    {
      var config = new RegressionTestConfiguration("TestConfig", RegressionTestKind.EqualityTest)
      {
        Inifile = "config.ini",
        Sourcefile = "input.txt",
        Outfile = "output.txt",
        Logfile = "test.log",
        AdditionalParameter = new Dictionary<string, string>
        {
          { "verbose", "true" },
          { "debug", "false" }
        }
      };

      var commandString = config.GetCommandStringForPythonScript();

      Assert.That(commandString, Does.Contain("-i \"input.txt\""));
      Assert.That(commandString, Does.Contain("-o \"output.txt\""));
      Assert.That(commandString, Does.Contain("-g \"test.log\""));
      Assert.That(commandString, Does.Contain("-ini \"config.ini\""));
      Assert.That(commandString, Does.Contain("-verbose \"true\""));
      Assert.That(commandString, Does.Contain("-debug \"false\""));
    }

    [Test]
    public void GetCommandStringForPythonScript_WithEmptyParameters_GeneratesEmptyString()
    {
      var config = new RegressionTestConfiguration("TestConfig", RegressionTestKind.EqualityTest);

      var commandString = config.GetCommandStringForPythonScript();

      Assert.That(commandString, Is.EqualTo(""));
    }

    [Test]
    public void GetCommandStringForPythonScript_WithOnlySourcefile_GeneratesCorrectString()
    {
      var config = new RegressionTestConfiguration("TestConfig", RegressionTestKind.EqualityTest)
      {
        Sourcefile = "input.txt"
      };

      var commandString = config.GetCommandStringForPythonScript();

      Assert.That(commandString, Is.EqualTo("-i \"input.txt\""));
    }

    #endregion

    #region Helper Methods

    private string CreateBasicXmlWithEqualityTest()
    {
      return @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";
    }

    private string CreateBasicXmlWithFailTest()
    {
      return @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <FAILTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <FAILTEST Name=""FailTest1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </FAILTEST>
          </FAILTESTS>
        </root>";
    }

    private string CreateXmlWithIniFiles()
    {
      return @"
        <root>
          <INIFILES>
            <SOURCEDIR>" + mTestRootDir + @"/configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""Test1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
        </root>";
    }

    private string CreateXmlWithMultipleTestTypes()
    {
      return @"
        <root>
          <INIFILES>
            <SOURCEDIR>configs</SOURCEDIR>
            <INIFILE id=""ini1"">config1.ini</INIFILE>
          </INIFILES>
          <EQUALITYTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <EQUALITYTEST Name=""EqualTest1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </EQUALITYTEST>
          </EQUALITYTESTS>
          <FAILTESTS>
            <SOURCEDIR>source</SOURCEDIR>
            <TARGETDIR>target</TARGETDIR>
            <FAILTEST Name=""FailTest1"" inifile=""ini1"">
              <SOURCEFILES><SOURCEFILE>input.txt</SOURCEFILE></SOURCEFILES>
              <OUTFILES><OUTFILE>output.txt</OUTFILE></OUTFILES>
            </FAILTEST>
          </FAILTESTS>
        </root>";
    }

    #endregion
  }
}
