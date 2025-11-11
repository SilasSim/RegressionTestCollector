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
    public void Constructor_SetsInitialValues()
    {
      var sut = new MatchCounter(3, 7);

      Assert.That(sut.VisibleCounter, Is.EqualTo(3));
      Assert.That(sut.TotalCounter, Is.EqualTo(7));
    }

    [Test]
    public void Constructor_WithZeroValues_HandlesCorrectly()
    {
      var sut = new MatchCounter(0, 0);

      Assert.That(sut.VisibleCounter, Is.EqualTo(0));
      Assert.That(sut.TotalCounter, Is.EqualTo(0));
      Assert.That(sut.MatchCounterText, Is.EqualTo("0 / 0"));
    }

    [Test]
    public void Constructor_WithNegativeValues_AcceptsValues()
    {
      var sut = new MatchCounter(-1, -5);

      Assert.That(sut.VisibleCounter, Is.EqualTo(-1));
      Assert.That(sut.TotalCounter, Is.EqualTo(-5));
      Assert.That(sut.MatchCounterText, Is.EqualTo("-1 / -5"));
    }

    #endregion

    #region MatchCounterText Property Tests

    [Test]
    public void MatchCounterText_ReturnsCorrectFormat()
    {
      Assert.That(mSut.MatchCounterText, Is.EqualTo("5 / 10"));
    }

    [Test]
    public void MatchCounterText_UpdatesWhenVisibleCounterChanges()
    {
      mSut.VisibleCounter = 8;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("8 / 10"));
    }

    [Test]
    public void MatchCounterText_UpdatesWhenTotalCounterChanges()
    {
      mSut.TotalCounter = 15;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("5 / 15"));
    }

    [Test]
    public void MatchCounterText_UpdatesWhenBothCountersChange()
    {
      mSut.VisibleCounter = 12;
      mSut.TotalCounter = 20;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("12 / 20"));
    }

    #endregion

    #region Property Change Notification Tests

    [Test]
    public void VisibleCounter_PropertyChanged_TriggersNotification()
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
    public void TotalCounter_PropertyChanged_TriggersNotification()
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
    public void VisibleCounter_Change_TriggersMatchCounterTextNotification()
    {
      var matchCounterTextChanged = false;

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(MatchCounter.MatchCounterText))
          matchCounterTextChanged = true;
      };

      mSut.VisibleCounter = 3;

      Assert.That(matchCounterTextChanged, Is.True);
    }

    [Test]
    public void TotalCounter_Change_TriggersMatchCounterTextNotification()
    {
      var matchCounterTextChanged = false;

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(MatchCounter.MatchCounterText))
          matchCounterTextChanged = true;
      };

      mSut.TotalCounter = 8;

      Assert.That(matchCounterTextChanged, Is.True);
    }

    [Test]
    public void PropertyChanged_MultipleEvents_WhenSettingBothCounters()
    {
      var eventsFired = new List<string>();

      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName != null)
          eventsFired.Add(e.PropertyName);
      };

      mSut.VisibleCounter = 6;
      mSut.TotalCounter = 11;

      // Should fire: VisibleCounter, MatchCounterText, TotalCounter, MatchCounterText
      Assert.That(eventsFired.Count, Is.EqualTo(4));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.VisibleCounter)));
      Assert.That(eventsFired, Contains.Item(nameof(MatchCounter.TotalCounter)));
      Assert.That(eventsFired.Count(x => x == nameof(MatchCounter.MatchCounterText)), Is.EqualTo(2));
    }

    [Test]
    public void PropertyChanged_NotTriggeredWhenSettingSameValue()
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
    public void VisibleCounter_GreaterThanTotal_AllowedAndFormatsCorrectly()
    {
      mSut.VisibleCounter = 15;
      mSut.TotalCounter = 10;

      Assert.That(mSut.MatchCounterText, Is.EqualTo("15 / 10"));
    }

    [Test]
    public void Counters_SetToMaxValue_HandlesCorrectly()
    {
      mSut.VisibleCounter = int.MaxValue;
      mSut.TotalCounter = int.MaxValue;

      Assert.That(mSut.MatchCounterText, Is.EqualTo($"{int.MaxValue} / {int.MaxValue}"));
    }

    [Test]
    public void Counters_SetToMinValue_HandlesCorrectly()
    {
      mSut.VisibleCounter = int.MinValue;
      mSut.TotalCounter = int.MinValue;

      Assert.That(mSut.MatchCounterText, Is.EqualTo($"{int.MinValue} / {int.MinValue}"));
    }
    #endregion
  }
}