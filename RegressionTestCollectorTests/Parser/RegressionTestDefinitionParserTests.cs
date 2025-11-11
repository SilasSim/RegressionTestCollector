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
    public void Parse_WithValidXmlContainingNameAndServer_ReturnsCorrectData()
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
    public void Parse_WithMissingNameElement_ReturnsEmptyName()
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
    public void Parse_WithMissingDefaultServerParameter_ReturnsEmptyDefaultServerString()
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
    public void Parse_WithEmptyXml_ReturnsEmptyValues()
    {

      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<TESTDEFINITION>
</TESTDEFINITION>";


      var result = suv.Parse(xml);

      Assert.That(result.Name, Is.EqualTo(""));
      Assert.That(result.ServerDefaultValue, Is.EqualTo(""));
    }

    [Test]
    public void Parse_WithServerParameterWithoutDefaultValue_ReturnsEmptyString()
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
    public void Parse_WithEmptyServerParameterContent_ReturnsEmptyString()
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
    public void Parse_WithInvalidXml_ThrowsException()
    {
      // Arrange
      var invalidXml = @"<ROOT><NAME>Unclosed";

      // Act & Assert
      Assert.Throws<System.Xml.XmlException>(() => suv.Parse(invalidXml));
    }

  }
}
