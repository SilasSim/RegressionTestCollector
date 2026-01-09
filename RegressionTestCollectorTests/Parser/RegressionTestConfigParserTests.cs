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
    public void GivenValidXmlWithEqualityTests_WhenParseIsCalled_ThenReturnsCorrectRegressionTestDat()
    {
      var xmlData = CreateBasicXmlWithEqualityTest();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Kind, Is.EqualTo(RegressionTestKind.EqualityTest));
      Assert.That(result.RegressionTests[0].Name, Is.EqualTo("Test1"));
    }

    [Test]
    public void GivenValidXmlWithFailTests_WhenParseIsCalled_ThenReturnsCorrectRegressionTestDat()
    {
      var xmlData = CreateBasicXmlWithFailTest();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Kind, Is.EqualTo(RegressionTestKind.FailTest));
      Assert.That(result.RegressionTests[0].Name, Is.EqualTo("FailTest1"));
    }

    [Test]
    public void GivenEmptyXml_WhenParseIsCalled_ThenReturnsEmptyResult()
    {
      var xmlData = "<root></root>";

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.RegressionTests, Is.Empty);
    }

    [Test]
    public void GivenInvalidXml_WhenParseIsCalled_ThenThrowsXmlException()
    {
      var xmlData = "<invalid><unclosed>";

      Assert.Throws<System.Xml.XmlException>(() => mSut.Parse(xmlData, mTestRootDir));
    }

    #endregion

    #region INI File Handling Tests

    [Test]
    public void GivenXmlWithIniFiles_WhenParseIsCalled_ThenMapsIniFilePathCorrectly()
    {
      var xmlData = CreateXmlWithIniFiles();

      var result = mSut.Parse(xmlData, mTestRootDir);

      Assert.That(result.RegressionTests, Has.Count.EqualTo(1));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain(mTestRootDir));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain("configs"));
      Assert.That(result.RegressionTests[0].Inifile, Does.Contain("config1.ini"));
    }

    [Test]
    public void GivenXmlWithRootDirPlaceholders_WhenParseIsCalled_ThenReplacesPathsWithRootDir()
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
    public void GivenXmlReferencingMissingIniId_WhenParseIsCalled_ThenThrowsKeyNotFoundException()
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
    public void GivenXmlWithMultipleTestTypes_WhenParseIsCalled_ThenParsesEqualityAndFailTests()
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
    public void GivenXmlWithMultipleEqualityTests_WhenParseIsCalled_ThenParsesAllTests()
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
    public void GivenXmlWithAdditionalParameters_WhenParseIsCalled_ThenMapsParametersToDictionary()
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
    public void GivenXmlWithEmptyAdditionalParameters_WhenParseIsCalled_ThenAdditionalParameterIsEmpty()
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
    public void GivenXmlMissingTestName_WhenParseIsCalled_ThenSkipsTest()
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
    public void GivenXmlWithoutSourceFiles_WhenParseIsCalled_ThenCreatesTestWithEmptySourcePath()
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
    public void GivenXmlWithLogFile_WhenParseIsCalled_ThenSetsLogfileToSpecifiedValue()
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
