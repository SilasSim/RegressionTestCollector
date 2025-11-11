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
  public class FilterSetTests
  {
    private FilterSet mSut;

    [SetUp]
    public void Setup()
    {
      mSut = new FilterSet();
    }

    #region FilterSet Add Method Tests

    [Test]
    public void Add_NewElement_AddsToCollection()
    {
      var element = new FilterElement("TestFilter");

      mSut.Add(element);

      Assert.That(mSut.Count, Is.EqualTo(1));
      Assert.That(mSut[0], Is.EqualTo(element));
      Assert.That(mSut[0].Text, Is.EqualTo("TestFilter"));
    }

    [Test]
    public void Add_MultipleUniqueElements_AddsAll()
    {
      var element1 = new FilterElement("Filter1");
      var element2 = new FilterElement("Filter2");
      var element3 = new FilterElement("Filter3");

      mSut.Add(element1);
      mSut.Add(element2);
      mSut.Add(element3);

      Assert.That(mSut.Count, Is.EqualTo(3));
      Assert.That(mSut, Contains.Item(element1));
      Assert.That(mSut, Contains.Item(element2));
      Assert.That(mSut, Contains.Item(element3));
    }

    [Test]
    public void Add_DuplicateElement_DoesNotAdd()
    {
      var element1 = new FilterElement("SameText");
      var element2 = new FilterElement("SameText");

      mSut.Add(element1);
      mSut.Add(element2);

      Assert.That(mSut.Count, Is.EqualTo(1));
      Assert.That(mSut[0], Is.EqualTo(element1));
    }

    [Test]
    public void Add_SameTextDifferentObjects_DoesNotAdd()
    {
      var element1 = new FilterElement("DuplicateText");
      var element2 = new FilterElement("DuplicateText");

      mSut.Add(element1);
      mSut.Add(element2);

      Assert.That(mSut.Count, Is.EqualTo(1));
      Assert.That(mSut[0], Is.EqualTo(element1));
      Assert.That(mSut[0], Is.Not.EqualTo(element2));
    }

    [Test]
    public void Add_CaseSensitive_AddsBothElements()
    {
      var element1 = new FilterElement("filter");
      var element2 = new FilterElement("Filter");

      mSut.Add(element1);
      mSut.Add(element2);

      Assert.That(mSut.Count, Is.EqualTo(2));
      Assert.That(mSut, Contains.Item(element1));
      Assert.That(mSut, Contains.Item(element2));
    }

    [Test]
    public void Add_NullText_HandlesGracefully()
    {
      var element = new FilterElement(null!);

      mSut.Add(element);

      Assert.That(mSut.Count, Is.EqualTo(1));
      Assert.That(mSut[0].Text, Is.Null);
    }

    [Test]
    public void Add_WithWhitespace_TreatsAsUnique()
    {
      var element1 = new FilterElement("filter");
      var element2 = new FilterElement(" filter ");
      var element3 = new FilterElement("filter ");

      mSut.Add(element1);
      mSut.Add(element2);
      mSut.Add(element3);

      Assert.That(mSut.Count, Is.EqualTo(3));
    }

    #endregion

  }

  [TestFixture]
  public class FilterElementTests
  {
    #region Constructor Tests

    [Test]
    public void Constructor_WithTextOnly_SetsTextAndDefaultChecked()
    {
      var sut = new FilterElement("TestText");

      Assert.That(sut.Text, Is.EqualTo("TestText"));
      Assert.That(sut.IsChecked, Is.False);
    }

    [Test]
    public void Constructor_WithTextAndChecked_SetsBothProperties()
    {
      var sut = new FilterElement("TestText", true);

      Assert.That(sut.Text, Is.EqualTo("TestText"));
      Assert.That(sut.IsChecked, Is.True);
    }

    #endregion

    #region Property Tests

    [Test]
    public void IsChecked_CanBeToggled()
    {
      var sut = new FilterElement("Test");

      sut.IsChecked = true;
      Assert.That(sut.IsChecked, Is.True);

      sut.IsChecked = false;
      Assert.That(sut.IsChecked, Is.False);
    }
    #endregion
  }
}