using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RegressionTestCollector.Themes;
using RegressionTestCollector.ViewModel;
using RegressionTestCollectorTests.TestUtils;

namespace RegressionTestCollectorTests.ViewModel
{
  [TestFixture]
  public class MenuViewModelTests
  {
    private MenuViewModel mSut;
    private TestSettingsService mTestSettingsService;
    private Mock<IThemeService> mMockThemeService;

    [SetUp]
    public void SetUp()
    {
      mTestSettingsService = new TestSettingsService();
      mMockThemeService = new Mock<IThemeService>();
      mSut = new MenuViewModel(mTestSettingsService, mMockThemeService.Object);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesPropertiesFromSettingsService()
    {
      mTestSettingsService.SelectedTheme = "TestTheme";
      mTestSettingsService.SelectedPythonCommand = "TestPythonCommand";
      mTestSettingsService.SelectedStrategy = "TestStrategy";

      mSut = new MenuViewModel(mTestSettingsService, mMockThemeService.Object);

      Assert.That(mSut.SelectedTheme, Is.EqualTo("TestTheme"));
      Assert.That(mSut.SelectedPythonCommand, Is.EqualTo("TestPythonCommand"));
      Assert.That(mSut.SelectedStrategy, Is.EqualTo("TestStrategy"));
    }

    [Test]
    public void GivenNewTheme_WhenSelectedThemeIsSet_ThenUpdatesSettingsService()
    {
      var newTheme = "Pink";
      mSut.SelectedTheme = newTheme;
      Assert.That(mSut.SelectedTheme, Is.EqualTo(newTheme));
      Assert.That(mTestSettingsService.SelectedTheme, Is.EqualTo(newTheme));
    }

    [Test]
    public void GivenNewPythonCommand_WhenSelectedPythonCommandIsSet_ThenUpdatesSettingsService()
    {
      var newCommand = "python4";
      mSut.SelectedPythonCommand = newCommand;
      Assert.That(mSut.SelectedPythonCommand, Is.EqualTo(newCommand));
      Assert.That(mTestSettingsService.SelectedPythonCommand, Is.EqualTo(newCommand));
    }

    [Test]
    public void GivenNewStrategy_WhenSelectedStrategyIsSet_ThenUpdatesSettingsService()
    {
      var newStrategy = "Rust";
      mSut.SelectedStrategy = newStrategy;
      Assert.That(mSut.SelectedStrategy, Is.EqualTo(newStrategy));
      Assert.That(mTestSettingsService.SelectedStrategy, Is.EqualTo(newStrategy));
    }

    [Test]
    public void WhenInitializeThemeIsCalled_ThenAppliesThemeOnThemeService()
    {
      var expectedTheme = mSut.SelectedTheme;
      mSut.InitializeTheme();
      mMockThemeService.Verify(x => x.ApplyTheme(expectedTheme), Times.Once);
    }
  }
}
