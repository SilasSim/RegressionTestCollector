using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.Models
{
  [TestFixture]
  public class LoadingProgressTests
  {
    private LoadingProgress mSut;

    [SetUp]
    public void Setup()
    {
      mSut = new LoadingProgress(10, 100);
    }
  }
}
