using RegressionTestCollector.Models;

namespace RegressionTestCollector.Parser
{
  public class RegressionTestParser
  {
    public IDataParser<Dictionary<string, string>> BatParser { get; } = new BatParser("regtest.exe");
    public IDataParser<RegressionTestConfigDataObject> ConfigParser { get; } = new RegressionTestConfigParser();
    public IDataParser<RegressionTestDefinitionDataObject> DefinitionParser { get; } =
      new RegressionTestDefinitionParser();
  }
}
