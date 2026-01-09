using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.Models;
using RegressionTestCollector.Utils;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollectorTests.ViewModel
{
  [TestFixture]
  public class OutputControlViewModelTests
  {
    private OutputControlViewModel mSut;

    [SetUp]
    public void SetUp()
    {
      mSut = new OutputControlViewModel();
    }


    [Test]
    public void WhenConstructorIsCalled_ThenOutputListIsEmpty()
    {
      Assert.That(mSut.OutputList, Has.Count.EqualTo(0));
    }

    [Test]
    public void GivenOutputListWithItems_WhenClearListCommandIsExecuted_ThenClearsOutputList()
    {
      mSut.OutputList.Add(new OutputData("Test"));
      mSut.OutputList.Add(new OutputData("Test2"));
      Assert.That(mSut.OutputList, Has.Count.EqualTo(2));
      mSut.ClearListCommand.Execute(null);
      Assert.That(mSut.OutputList, Has.Count.EqualTo(0));
    }

    [Test]
    public void WhenToggleVisibilityCommandIsExecuted_ThenTogglesVisibility()
    {
      Assert.That(mSut.IsVisible, Is.False);
      mSut.ToggleVisibilityCommand.Execute(null);
      Assert.That(mSut.IsVisible, Is.True);
      mSut.ToggleVisibilityCommand.Execute(null);
      Assert.That(mSut.IsVisible, Is.False);
    }

    [Test]
    public void GivenVisibilityChangedEvent_WhenIsVisibleIsSet_ThenVisibilityChangedIsFired()
    {
      var eventFired = false;
      mSut.VisibilityChanged += ((sender, obj) => eventFired = true);
      Assert.That(eventFired, Is.False);
      mSut.IsVisible = true;
      Assert.That(eventFired, Is.True);
    }

  }
}
