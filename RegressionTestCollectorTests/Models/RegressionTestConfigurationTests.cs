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
  public class RegressionTestConfigurationTests
  {

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void GivenEqualityTestConfiguration_WhenConstructorIsCalled_ThePropertiesAreSetCorrectly()
    {
      var sut = SetupEqualityTestConfiguration();

      Assert.That(sut.Name, Is.EqualTo("TEST"));
      Assert.That(sut.Kind, Is.EqualTo(RegressionTestKind.EqualityTest));
      Assert.That(sut.Sourcefile, Is.EqualTo(String.Empty));
      Assert.That(sut.Logfile, Is.EqualTo(String.Empty));
      Assert.That(sut.Inifile, Is.EqualTo(String.Empty));
      Assert.That(sut.Outfile, Is.EqualTo(String.Empty));
      Assert.That(sut.AdditionalParameter, Is.Empty);
    }

    [Test]
    public void GivenFailTestConfiguration_WhenConstructorIsCalled_ThePropertiesAreSetCorrectly()
    {
      var sut = SetupFailTestConfiguration();

      Assert.That(sut.Name, Is.EqualTo("TEST"));
      Assert.That(sut.Kind, Is.EqualTo(RegressionTestKind.FailTest));
      Assert.That(sut.Sourcefile, Is.EqualTo(String.Empty));
      Assert.That(sut.Logfile, Is.EqualTo(String.Empty));
      Assert.That(sut.Inifile, Is.EqualTo(String.Empty));
      Assert.That(sut.Outfile, Is.EqualTo(String.Empty));
      Assert.That(sut.AdditionalParameter, Is.Empty);
    }

    [Test]
    public void GivenSuccessTestConfiguration_WhenConstructorIsCalled_ThePropertiesAreSetCorrectly()
    {
      var sut = SetupSuccessTestConfiguration();

      Assert.That(sut.Name, Is.EqualTo("TEST"));
      Assert.That(sut.Kind, Is.EqualTo(RegressionTestKind.SuccessTest));
      Assert.That(sut.Sourcefile, Is.EqualTo(String.Empty));
      Assert.That(sut.Logfile, Is.EqualTo(String.Empty));
      Assert.That(sut.Inifile, Is.EqualTo(String.Empty));
      Assert.That(sut.Outfile, Is.EqualTo(String.Empty));
      Assert.That(sut.AdditionalParameter, Is.Empty);
    }

    [Test]
    public void GivenEmptyProperties_WhenGetCommandStringForPythonScriptIsCalled_ThenReturnsEmptyString()
    {
      var sut = SetupEqualityTestConfiguration();

      Assert.That(sut.GetCommandStringForPythonScript(), Is.Empty);
    }

    [Test]
    public void GivenSetPropertiesAndEmptyAdditionalParameters_WhenGetCommandStringForPythonScriptIsCalled_ThenReturnsCorrectString()
    {
      var sut = SetupEqualityTestConfiguration();
      sut.Inifile = "INIFILE";
      sut.Outfile = "OUTFILE";
      sut.Sourcefile = "SOURCE";
      sut.Logfile = "LOGFILE";

      Assert.That(sut.GetCommandStringForPythonScript(), Is.EqualTo(
        "-i \"SOURCE\" -o \"OUTFILE\" -g \"LOGFILE\" -ini \"INIFILE\""));
    }

    [Test]
    public void GivenAdditionalParametersAndEmptyProperties_WhenGetCommandStringForPythonScriptIsCalled_ThenReturnsCorrectString()
    {
      var sut = SetupEqualityTestConfiguration();
      sut.AdditionalParameter["TEST1"] = "someString1";
      sut.AdditionalParameter["TEST2"] = "someString2";

      Assert.That(sut.GetCommandStringForPythonScript(), Is.EqualTo("-TEST1 \"someString1\" -TEST2 \"someString2\""));
    }

    [Test]
    public void GivenSetPropertiesAndAdditionalParameters_WhenGetCommandStringForPythonScriptIsCalled_ThenReturnsCorrectString()
    {
      var sut = SetupEqualityTestConfiguration();
      sut.Inifile = "INIFILE";
      sut.Outfile = "OUTFILE";
      sut.Sourcefile = "SOURCE";
      sut.Logfile = "LOGFILE";
      sut.AdditionalParameter["TEST1"] = "someString1";
      sut.AdditionalParameter["TEST2"] = "someString2";

      Assert.That(sut.GetCommandStringForPythonScript(), Is.EqualTo(
        "-i \"SOURCE\" -o \"OUTFILE\" -g \"LOGFILE\" -ini \"INIFILE\" -TEST1 \"someString1\" -TEST2 \"someString2\""));
    }


    private RegressionTestConfiguration SetupEqualityTestConfiguration()
    {
      return new RegressionTestConfiguration("TEST", RegressionTestKind.EqualityTest);
    }

    private RegressionTestConfiguration SetupFailTestConfiguration()
    {
      return new RegressionTestConfiguration("TEST", RegressionTestKind.FailTest);
    }

    private RegressionTestConfiguration SetupSuccessTestConfiguration()
    {
      return new RegressionTestConfiguration("TEST", RegressionTestKind.SuccessTest);
    }

  }

}