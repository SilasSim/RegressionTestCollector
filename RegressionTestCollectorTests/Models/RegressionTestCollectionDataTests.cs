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
  public class RegressionTestCollectionDataTests
  {
    private RegressionTestCollectionData mSut;

    [SetUp]
    public void Setup()
    {
      mSut = new RegressionTestCollectionData();
    }


    [Test]
    public void EmptyErrorList_HasErrorReturnsFalse()
    {
      Assert.That(mSut.HasError, Is.False);
    }

    [Test]
    public void Add_ErrorObject_HasErrorReturnsTrue()
    {
      mSut.Errors.Add(new ErrorObject("TEST"));

      Assert.That(mSut.Errors.Count, Is.EqualTo(1));
      Assert.That(mSut.Data.Count, Is.EqualTo(0));
      Assert.That(mSut.HasError, Is.True);
    }

    [Test]
    public void Add_DataElement_HasErrorReturnsTrue()
    {
      mSut.Data.Add(new RegressionTestDataObject("TEST"));

      Assert.That(mSut.Errors.Count, Is.EqualTo(0));
      Assert.That(mSut.Data.Count, Is.EqualTo(1));
      Assert.That(mSut.HasError, Is.False);
    }

    [Test]
    public void AddThenRemove_ErrorObject_HasErrorReturnsFalse()
    {
      var error = new ErrorObject("TEST");
      mSut.Errors.Add(error);
      mSut.Errors.Remove(error);

      Assert.That(mSut.Errors.Count, Is.EqualTo(0));
      Assert.That(mSut.HasError, Is.False);
    }

  }

}