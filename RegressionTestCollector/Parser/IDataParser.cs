namespace RegressionTestCollector.Parser
{
  public interface IDataParser<out T> where T : class
  {
    T Parse(string data, string rootDir = "");
  }
}
