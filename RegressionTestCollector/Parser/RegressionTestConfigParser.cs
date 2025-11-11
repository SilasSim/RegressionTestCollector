using System.Diagnostics;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Xml.XPath;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.Parser
{
  public class RegressionTestConfigParser : IDataParser<RegressionTestConfigDataObject>
  {
    public string RootDir { get; private set; } = "";
    private XElement? mRoot;
    private readonly Dictionary<string, string> Inifiles = new();
    public RegressionTestConfigDataObject Parse(string data, string rootDir = "")
    {
      RootDir = rootDir;
      var returnObject = new RegressionTestConfigDataObject();
      XDocument doc = XDocument.Parse(data);
      mRoot = doc.Root;

      GetInifiles();

      returnObject.RegressionTests.AddRange(GetTests(RegressionTestKind.EqualityTest));
      returnObject.RegressionTests.AddRange(GetTests(RegressionTestKind.FailTest));
      //returnObject.RegressionTests.AddRange(GetTests(root, RegressionTestKind.SuccessTest));

      return returnObject;
    }

    private void GetInifiles()
    {
      if (mRoot is null)
      {
        return;
      }

      string sourceDir = mRoot?.XPathSelectElement("//INIFILES/SOURCEDIR")?.Value ?? "";
      sourceDir = sourceDir.Replace("%ROOTDIR;", RootDir);
      var inifileElements = mRoot?.XPathSelectElements("//INIFILES/INIFILE");

      if (inifileElements is not null)
      {
        foreach (var inifileElement in inifileElements)
        {
          string id = inifileElement.Attribute("id")?.Value ?? "";
          string filename = inifileElement.Value;

          if (!String.IsNullOrWhiteSpace(id)) Inifiles[id] = Path.Join(sourceDir, filename);
        }
      }


    }

    private List<RegressionTestConfiguration> GetTests(RegressionTestKind kind)
    {
      return kind switch
      {
        RegressionTestKind.EqualityTest => GetTests(kind, "EQUALITYTESTS"),
        RegressionTestKind.FailTest => GetTests(kind, "FAILTESTS"),
        RegressionTestKind.SuccessTest => GetTests(kind, "SUCCESSTESTS"),
        _ => new List<RegressionTestConfiguration>()
      };
    }

    private List<RegressionTestConfiguration> GetTests(RegressionTestKind kind, string kindTag)
    {
      var tests = new List<RegressionTestConfiguration>();

      var testsContainer = mRoot?.Element(kindTag);

      var sourcedir = testsContainer?.Element("SOURCEDIR")?.Value;
      sourcedir = sourcedir?.Replace("%ROOTDIR;", RootDir);
      var targetDir = testsContainer?.Element("TARGETDIR")?.Value;
      targetDir = targetDir?.Replace("%ROOTDIR;", RootDir);
      var referenceDir = testsContainer?.Element("REFERENCEDIR")?.Value;

      var testElements = testsContainer?.Elements($"{kindTag.Substring(0, kindTag.Length - 1)}") ?? [];

      foreach (var testElem in testElements)
      {
        var inifileid = testElem.Attribute("inifile")?.Value ?? "";
        var name = testElem.Attribute("Name")?.Value;

        if (name is null)
        {
          continue;
        }

        var additionalParams = new Dictionary<string, string>();

        var additionalParamElements = testElem.XPathSelectElements("./ADDITIONAL-PARAMETERS/PARAMETER");

        foreach (var additionalParam in additionalParamElements)
        {
          var value = additionalParam.Attribute("value")?.Value;
          var key = additionalParam.Value;
          if (value is not null)
          {
            additionalParams[key] = value;
          }
        }

        var sourcefile = testElem.Element("SOURCEFILES")?.Element("SOURCEFILE")?.Value ?? "";
        var outfile = testElem.Element("OUTFILES")?.Element("OUTFILE")?.Value ?? "";
        var logfile = testElem.Element("LOGFILE")?.Value ?? "";

        tests.Add(new RegressionTestConfiguration(name, kind)
        {
          Inifile = Inifiles[inifileid],
          Sourcefile = Path.Join(sourcedir, sourcefile),
          Outfile = Path.Join(targetDir, outfile),
          Logfile = logfile,
          AdditionalParameter = additionalParams
        });
      }

      return tests;
    }
  }
}
