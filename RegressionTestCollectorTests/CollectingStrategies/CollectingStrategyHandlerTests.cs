using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.CollectingStrategies;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.CollectingStrategies
{
  [TestFixture]
  public class CollectingStrategyHandlerTests
  {
    private CollectingStrategyHandler mSut;
    private TestCollectingStrategy mTestStrategy;

    [SetUp]
    public void Setup()
    {
      mTestStrategy = new TestCollectingStrategy();
      mSut = new CollectingStrategyHandler(mTestStrategy);
    }

    [Test]
    public void WhenConstructorIsCalled_CollectingStrategyIsSet()
    {
      Assert.That(mSut.CollectingStrategy, Is.EqualTo(mTestStrategy));
    }

    [Test]
    public void GivenPythonCommand_WhenCollectIsCalled_ThenStrategyIsUsingThePythonCommand()
    {
      mSut.Collect("python3");

      Assert.That(mTestStrategy.CollectCount, Is.EqualTo(1));
      Assert.That(mTestStrategy.LastPythonCommand, Is.EqualTo("python3"));
    }

    [Test]
    public void WhenCollectIsCalled_ThenCollectOfTheStrategyIsCalled()
    {
      var expectedData = new RegressionTestCollectionData();
      expectedData.Errors.Add(new ErrorObject("ERROR"));
      expectedData.Data.Add(new RegressionTestDataObject("TEST"));
      mTestStrategy.Testdata = expectedData;
      var result = mSut.Collect("python2");

      Assert.That(result, Is.EqualTo(expectedData));
    }

    [Test]
    public void GivenCollectringStrategy_WhenCollectingStrategyIsChanged_ThenSetsNewCollectingStrategy()
    {
      var newStrategy = new TestCollectingStrategy();
      mSut.ChangeCollectingStrategy(newStrategy);
      Assert.That(mSut.CollectingStrategy, Is.EqualTo(newStrategy));
    }

    private class TestCollectingStrategy : ICollectingStrategy
    {
      public string? LastPythonCommand { get; private set; }
      public int CollectCount { get; private set; }
      public RegressionTestCollectionData? Testdata { get; set; }
      public RegressionTestCollectionData Collect(string pythonCommand)
      {
        LastPythonCommand = pythonCommand;
        CollectCount++;
        return Testdata ?? new RegressionTestCollectionData();
      }
    }

  }

}