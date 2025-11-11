using RegressionTestCollector.Models;

namespace RegressionTestCollector.CollectingStrategies
{
  public interface ICollectingStrategy
  {
    public RegressionTestCollectionData Collect(string pythonCommand);
  }
}
