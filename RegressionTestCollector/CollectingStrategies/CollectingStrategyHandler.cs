using RegressionTestCollector.Models;

namespace RegressionTestCollector.CollectingStrategies
{
  public class CollectingStrategyHandler
  {
    public ICollectingStrategy CollectingStrategy { get; private set; }

    public CollectingStrategyHandler(ICollectingStrategy collectingStrategy)
    {
      CollectingStrategy = collectingStrategy;
    }

    public void ChangeCollectingStrategy(ICollectingStrategy collectingStrategy)
    {
      CollectingStrategy = collectingStrategy;
    }

    public RegressionTestCollectionData Collect(string pythonCommand)
    {
      return CollectingStrategy.Collect(pythonCommand);
    }

  }
}
