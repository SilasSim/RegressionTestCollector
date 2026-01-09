using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Parser;

namespace RegressionTestCollectorTests.Parser
{
  public class RegressionTestDefinitionParserTests
  {
    private RegressionTestDefinitionParser suv;
    [SetUp]
    public void Setup()
    {
      suv = new RegressionTestDefinitionParser();
    }

    [Test]
    public void GivenValidXmlWithNameAndServerParameter_WhenParseIsCalled_ThenReturnsNameAndServerDefaultValue()
    {

      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
    <NAME>TestName123</NAME>
    <PARAMETERS>
        <ADDITIONAL-PARAMETERS>
        <PARAMETER usage=""mandatory"" defaultValue=""TestPath"">server</PARAMETER>
        <PARAMETER usage=""mandatory"">scenario</PARAMETER>
      </ADDITIONAL-PARAMETERS>
    </PARAMETERS>
</TESTDEFINITION>";


      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo("TestName123"));
      Assert.That(result.ServerDefaultValue, Is.EqualTo("TestPath"));
    }

    [Test]
    public void GivenXmlMissingNameElement_WhenParseIsCalled_ThenReturnsEmptyName()
    {

      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
    <PARAMETERS>
        <ADDITIONAL-PARAMETERS>
        <PARAMETER usage=""mandatory"" defaultValue=""TestPath"">server</PARAMETER>
        <PARAMETER usage=""mandatory"">scenario</PARAMETER>
      </ADDITIONAL-PARAMETERS>
    </PARAMETERS>
</TESTDEFINITION>";


      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo(""));
      Assert.That(result.ServerDefaultValue, Is.EqualTo("TestPath"));
    }

    [Test]
    public void GivenXmlMissingDefaultServerParameter_WhenParseIsCalled_ThenReturnsEmptyServerDefaultValue()
    {

      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
    <NAME>TestName123</NAME>
    <PARAMETERS>
        <ADDITIONAL-PARAMETERS>
        <PARAMETER usage=""mandatory"">scenario</PARAMETER>
      </ADDITIONAL-PARAMETERS>
    </PARAMETERS>
</TESTDEFINITION>";


      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo("TestName123"));
      Assert.That(result.ServerDefaultValue, Is.EqualTo(""));
    }

    [Test]
    public void GivenEmptyTestDefinitionXml_WhenParseIsCalled_ThenReturnsEmptyValues()
    {

      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
</TESTDEFINITION>";


      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo(""));
      Assert.That(result.ServerDefaultValue, Is.EqualTo(""));
    }

    [Test]
    public void GivenServerParameterWithoutDefaultValue_WhenParseIsCalled_ThenReturnsEmptyServerDefaultValue()
    {
      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
    <NAME>TestName123</NAME>
    <PARAMETERS>
        <ADDITIONAL-PARAMETERS>
          <PARAMETER usage=""mandatory"">scenario</PARAMETER>
          <PARAMETER>server</PARAMETER>
      </ADDITIONAL-PARAMETERS>
    </PARAMETERS>
</TESTDEFINITION>";

      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo("TestName123"));
      Assert.That(result.ServerDefaultValue, Is.EqualTo(""));
    }

    [Test]
    public void GivenServerParameterWithEmptyContent_WhenParseIsCalled_ThenReturnsEmptyServerDefaultValue()
    {
      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
    <NAME>TestName123</NAME>
    <PARAMETERS>
        <ADDITIONAL-PARAMETERS>
          <PARAMETER usage=""mandatory"">scenario</PARAMETER>
          <PARAMETER defaultValue=""localhost""></PARAMETER>
      </ADDITIONAL-PARAMETERS>
    </PARAMETERS>
</TESTDEFINITION>";

      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo("TestName123"));
      Assert.That(result.ServerDefaultValue, Is.EqualTo(""));
    }

    [Test]
    public void GivenInvalidXml_WhenParseIsCalled_ThenThrowsXmlException()
    {
      var invalidXml = @"<ROOT><NAME>Unclosed";

      Assert.Throws<System.Xml.XmlException>(() => suv.Parse(invalidXml));
    }

  }
}
