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
  public class FilterControlViewModelTests
  {
    private FilterControlViewModel mSut;

    [SetUp]
    public void SetUp()
    {
      mSut = new FilterControlViewModel("Test");
    }


    [Test]
    public void WhenConstructorIsCalled_ThenNameIsSet()
    {
      Assert.That(mSut.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void GivenFilterSetWithCheckedElements_WhenUnselectAllIsExecuted_ThenUnchecksAllFilterElements()
    {
      mSut.FilterSet.Add(new FilterElement("el0", true));
      mSut.FilterSet.Add(new FilterElement("el1", true));
      mSut.FilterSet.Add(new FilterElement("el2", false));
      mSut.FilterSet.Add(new FilterElement("el3", true));


      mSut.UnselectAll.Execute(null);
      Assert.That(mSut.FilterSet.Any(x => x.IsChecked), Is.False);
      Assert.That(mSut.FilterAllElement.IsChecked, Is.False);
    }

    [Test]
    public void GivenFilterSetAndFilterAllElement_WhenFilterAllCommandIsExecuted_ThenSetsAllElementsToFilterAllElementState()
    {
      mSut.FilterSet.Add(new FilterElement("el0", true));
      mSut.FilterSet.Add(new FilterElement("el1", true));
      mSut.FilterSet.Add(new FilterElement("el2", false));
      mSut.FilterSet.Add(new FilterElement("el3", true));

      mSut.FilterAllCommand.Execute(null);

      Assert.That(mSut.FilterSet.Any(x => x.IsChecked), Is.False);
      Assert.That(mSut.FilterAllElement.IsChecked, Is.False);

      mSut.FilterAllElement.IsChecked = true;
      mSut.FilterAllCommand.Execute(null);
      Assert.That(mSut.FilterSet.Any(x => x.IsChecked), Is.True);
      Assert.That(mSut.FilterAllElement.IsChecked, Is.True);
    }

    [Test]
    public void GivenFilterSetWithGroups_WhenFilterGroupCommandIsExecuted_ThenUpdatesActiveGroupFilter()
    {
      var eventFired = false;
      mSut.FilterChanged += (sender, e) => eventFired = true;

      mSut.FilterSet.Add(new FilterElement("Group1", true));
      mSut.FilterSet.Add(new FilterElement("Group2", false));
      var elem3 = new FilterElement("Group3", true);
      mSut.FilterSet.Add(elem3);

      mSut.FilterGroupCommand.Execute(elem3);

      Assert.That(eventFired, Is.True);

      var activeGroups = mSut.GetActiveGroupFilter();
      Assert.That(activeGroups, Contains.Item("Group1"));
      Assert.That(activeGroups, Contains.Item("Group3"));
      Assert.That(activeGroups, Does.Not.Contain("Group2"));
      Assert.That(activeGroups.Count, Is.EqualTo(2));
    }

    [Test]
    public void GivenFilterSet_WhenFilterGroupCommandIsExecuted_ThenTriggersFilterChangedEvent()
    {
      var eventCount = 0;
      mSut.FilterChanged += (sender, e) => eventCount++;

      var elem1 = new FilterElement("Test", true);
      mSut.FilterSet.Add(elem1);

      mSut.FilterGroupCommand.Execute(elem1);

      Assert.That(eventCount, Is.EqualTo(1));
    }

  }
}
