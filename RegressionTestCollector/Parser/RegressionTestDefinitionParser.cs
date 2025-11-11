using System.Xml.Linq;
using System.Xml.XPath;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.Parser
{
  public class RegressionTestDefinitionParser : IDataParser<RegressionTestDefinitionDataObject>
  {
    public RegressionTestDefinitionDataObject Parse(string data, string rootDir = "")
    {
      XDocument doc = XDocument.Parse(data);
      XElement? root = doc.Root;

      string? testName = root?.Element("NAME")?.Value;

      var serverDefaultValue = root?.XPathSelectElement("//PARAMETER[text()='server']")?.Attribute("defaultValue")?.Value;

      return new RegressionTestDefinitionDataObject()
      {
        ServerDefaultValue = serverDefaultValue ?? "",
        Name = testName ?? ""
      };
    }
  }
}
