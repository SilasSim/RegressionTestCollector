using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RegressionTestCollector.Models;

namespace RegressionTestCollectorTests.Models
{
  [TestFixture]
  public class MatchCounterTests
  {
    private MatchCounter mSut;

    [SetUp]
    public void Setup()
    {
      mSut = new MatchCounter(5, 10);
    }

    #region Constructor Tests

    [Test]
    public void WhenConstructorIsCalled_ThenPropertiesAreSetCorrectly()
    {
      var sut = new MatchCounter(3, 7);

      Assert.That(sut.VisibleCounter, Is.EqualTo(3));
      Assert.That(sut.TotalCounter, Is.EqualTo(7));
    }

    [Test]
    public void GivenZeroValues_WhenConstructorIsCalled_ThenPropertiesAreSetCorrectly()
    {
      var sut = new MatchCounter(0, 0);

      Assert.That(sut.VisibleCounter, Is.EqualTo(0));
      Assert.That(sut.TotalCounter, Is.EqualTo(0));
      Assert.That(sut.MatchCounterText, Is.EqualTo("0 / 0"));
    }

    [Test]
    public void GivenNegativeValues_WhenConstructorIsCalled_ThenPropertiesAreSetCorrectly()
    {
      var sut = new MatchCounter(-1, -5);

      Assert.That(sut.VisibleCounter, Is.EqualTo(-1));
      Assert.That(sut.TotalCounter, Is.EqualTo(-5));
      Assert.That(sut.MatchCounterText, Is.EqualTo("-1 / -5"));
    }

    #endregion

    #region MatchCounterText Property Tests

    [Test]
    public void WhenMatchCounterTextIsAccessed_ThenReturnsFormattedText()
    {
      Assert.That(mSut.MatchCounterText, Is.EqualTo("5 / 10"));
    }

    [Test]
    public void WhenVisibleCounterIsChanged_ThenMatchCounterTextIsUpdated()
    {
      mSut.VisibleCounter = 8;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("8 / 10"));
    }

    [Test]
    public void WhenTotalCounterIsChanged_ThenMatchCounterTextIsUpdated()
    {
      mSut.TotalCounter = 15;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("5 / 15"));
    }

    [Test]
    public void WhenVisibleCounterAndTotalCounterAreChanged_ThenMatchCounterTextIsUpdated()
    {
      mSut.VisibleCounter = 12;
      mSut.TotalCounter = 20;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("12 / 20"));
    }

    #endregion

    #region Property Change Notification Tests

    [Test]
    public void WhenVisibleCounterValueChanges_ThenNotifiesVisibleCounterAndMatchCounterText()
    {
      var eventsFired = new List<string>();

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName != null)
          eventsFired.Add(e.PropertyName);
      };

      mSut.VisibleCounter = 7;

      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.VisibleCounter)));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.MatchCounterText)));
    }

    [Test]
    public void WhenTotalCounterValueChanges_ThenNotifiesTotalCounterAndMatchCounterText()
    {
      var eventsFired = new List<string>();

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName != null)
          eventsFired.Add(e.PropertyName);
      };

      mSut.TotalCounter = 12;

      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.TotalCounter)));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.MatchCounterText)));
    }

    [Test]
    public void WhenTotalAndVisibleCounterAreChanged_ThenNotifiesAllRelevantProperties()
    {
      var eventsFired = new List<string>();

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName != null)
          eventsFired.Add(e.PropertyName);
      };

      mSut.VisibleCounter = 6;
      mSut.TotalCounter = 11;

      Assert.That(eventsFired.Count, Is.EqualTo(4));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.VisibleCounter)));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.TotalCounter)));
      Assert.That(eventsFired.Count(x => x == nameof(MatchCounter.MatchCounterText)), Is.EqualTo(2));
    }

    [Test]
    public void WhenSameValueIsSet_ThenDoesNotRaisePropertyChanged()
    {
      var changeCount = 0;

      mSut.PropertyChanged += (sender, e) => changeCount++;

      mSut.VisibleCounter = 5;
      mSut.TotalCounter = 10;

      Assert.That(changeCount, Is.EqualTo(0));
    }

    #endregion

    #region Edge Cases Tests

    [Test]
    public void WhenVisibleCounterIsSetHigherThanTotalCounter_ThenAllowsItAndFormatsCorrectly()
    {
      mSut.VisibleCounter = 15;
      mSut.TotalCounter = 10;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("15 / 10"));
    }

    #endregion
  }
}