using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RegressionTestCollector.CollectingStrategies;
using RegressionTestCollector.Models;
using RegressionTestCollector.Resources;
using RegressionTestCollector.Themes;
using RegressionTestCollector.Utils;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollectorTests.ViewModel
{
  [TestFixture]
  [Apartment(ApartmentState.STA)]
  public class MainViewModelTests
  {
    private MainViewModel mSut;
    private TestSettingsService mTestSettingsService;
    private Mock<IThemeService> mMockThemeService;

    [SetUp]
    public void SetUp()
    {
      mTestSettingsService = new TestSettingsService();
      mMockThemeService = new Mock<IThemeService>();
      var menuViewModel = new MenuViewModel(mTestSettingsService, mMockThemeService.Object);
      var outputControlViewModel = new OutputControlViewModel();
      var searchTableViewModel = new SearchTableViewModel(outputControlViewModel, mTestSettingsService);
      mSut = new MainViewModel(menuViewModel, searchTableViewModel);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_InitializesChildVMs()
    {
      Assert.That(mSut.SearchTableViewModel, Is.Not.Null);
      Assert.That(mSut.MenuViewModel, Is.Not.Null);
      Assert.That(mSut.SearchTableViewModel.OutputControlViewModel, Is.Not.Null);
    }

    [Test]
    public void Constructor_InitializesChildCommands()
    {
      Assert.That(mSut.CancelLoadingCommand, Is.Not.Null);
      Assert.That(mSut.ChangeCollectingStrategyCommand, Is.Not.Null);
      Assert.That(mSut.ChangePythonCommandCommand, Is.Not.Null);
      Assert.That(mSut.ChangeThemeCommand, Is.Not.Null);
      Assert.That(mSut.LoadRegressionTestsCommand, Is.Not.Null);
    }

    [Test]
    public void Constructor_InitializesStrategyHandler()
    {
      Assert.That(mSut.CollectingStrategyHandler, Is.Not.Null);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.Not.Null);
    }

    [Test]
    public void Constructor_InitializesLoadingProgress()
    {
      Assert.That(mSut.LoadingProgress, Is.Not.Null);
      Assert.That(mSut.LoadingProgress.Current, Is.Zero);
    }

    #endregion

    #region Property Tests


    [Test]
    public void LoadingDataText_CanBeSetAndRetrieved()
    {
      mSut.LoadingDataText = "Test text";
      Assert.That(mSut.LoadingDataText, Is.EqualTo("Test text"));
    }

    [Test]
    public void LoadingDataText_HasDefaultValueOnStartup()
    {
      Assert.That(mSut.LoadingDataText, Is.Not.Null);
      Assert.That(mSut.LoadingDataText, Is.Not.Empty);
    }

    [Test]
    public void IsLoadingData_CanBeSetAndRetrieved()
    {
      var initVal = mSut.IsLoadingData;
      mSut.IsLoadingData = !initVal;
      Assert.That(mSut.IsLoadingData, Is.EqualTo(!initVal));
    }

    #endregion

    #region Command Tests

    [Test]
    public void ChangeThemeCommand_WithValidArgument_UpdatesMenuViewModel()
    {
      var newTheme = "Some Theme";
      mSut.ChangeThemeCommand.Execute(newTheme);
      Assert.That(mSut.MenuViewModel.SelectedTheme, Is.EqualTo(newTheme));
    }

    [Test]
    public void ChangeThemeCommand_WithInvalidArgument_DoesNothing()
    {
      var initTheme = mSut.MenuViewModel.SelectedTheme;
      mSut.ChangeThemeCommand.Execute(null);
      Assert.That(mSut.MenuViewModel.SelectedTheme, Is.EqualTo(initTheme));
    }

    [Test]
    public void ChangePythonCommandCommand_WithValidArgument_UpdatesMenuViewModel()
    {
      var newPythonCommand = "python2";
      mSut.ChangePythonCommandCommand.Execute(newPythonCommand);
      Assert.That(mSut.MenuViewModel.SelectedPythonCommand, Is.EqualTo(newPythonCommand));
    }

    [Test]
    public void ChangePythonCommandCommand_WithInvalidArgument_DoesNothing()
    {
      var initPythonCommand = mSut.MenuViewModel.SelectedPythonCommand;
      mSut.ChangePythonCommandCommand.Execute(null);
      Assert.That(mSut.MenuViewModel.SelectedPythonCommand, Is.EqualTo(initPythonCommand));
    }

    #endregion

    #region Strategy Change Tests

    [Test]
    public void ChangeCollectingStrategy_WithCSharpName_SetsCSharpStrategy()
    {
      var cSharpName = "C#";
      mSut.ChangeCollectingStrategyCommand.Execute(cSharpName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.TypeOf<CSharpCollectingStrategy>());
      Assert.That(mSut.MenuViewModel.SelectedStrategy, Is.EqualTo(cSharpName));
    }


    [Test]
    public void ChangeCollectingStrategy_WithPythonName_SetsPythonStrategy()
    {
      var pythonName = "Python";
      mSut.ChangeCollectingStrategyCommand.Execute(pythonName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.TypeOf<PythonCollectingStrategy>());
      Assert.That(mSut.MenuViewModel.SelectedStrategy, Is.EqualTo(pythonName));
    }


    [Test]
    public void ChangeCollectingStrategy_WithUnknownName_DoesNothing()
    {
      var originalStrategy = mSut.CollectingStrategyHandler.CollectingStrategy;

      var unknownName = "RandomTestStrategy";
      mSut.ChangeCollectingStrategyCommand.Execute(unknownName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.EqualTo(originalStrategy));
    }

    #endregion

    #region PropertyChanged Tests

    [Test]
    public void LoadingDataText_PropertyChanged_TriggersNotification()
    {
      var eventFired = false;
      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(MainViewModel.LoadingDataText))
          eventFired = true;
      };

      mSut.LoadingDataText = "New Text";
      Assert.That(eventFired, Is.True);
    }

    [Test]
    public void IsLoadingData_PropertyChanged_TriggersNotification()
    {
      var eventFired = false;
      mSut.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(MainViewModel.IsLoadingData))
          eventFired = true;
      };

      mSut.IsLoadingData = true;
      Assert.That(eventFired, Is.True);
    }

    #endregion

    #region LoadRegressionTests Tests

    [Test]
    public void LoadRegressionTestsCommand_CanExecute_ShouldReturnTrue()
    {
      Assert.That(mSut.LoadRegressionTestsCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void LoadRegressionTestsCommand_WhenStarted_SetsIsLoadingDataTrue()
    {
      var mockStrategy = new Mock<ICollectingStrategy>();
      mockStrategy.Setup(x => x.Collect(It.IsAny<string>())).Returns(new RegressionTestCollectionData()
      {
        Data = new List<RegressionTestDataObject>()
      });
      mSut.CollectingStrategyHandler.ChangeCollectingStrategy(mockStrategy.Object);

      mSut.LoadRegressionTestsCommand.Execute(null);
      Assert.That(mSut.IsLoadingData, Is.True);
    }

    [Test]
    public async Task LoadRegressionTestsCommand_WhenCompleted_SetsIsLoadingDataTrue()
    {
      var mockStrategy = new Mock<ICollectingStrategy>();
      mockStrategy.Setup(x => x.Collect(It.IsAny<string>())).Returns(new RegressionTestCollectionData()
      {
        Data = new List<RegressionTestDataObject>()
      });
      mSut.CollectingStrategyHandler.ChangeCollectingStrategy(mockStrategy.Object);

      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);
      Assert.That(mSut.IsLoadingData, Is.False);
    }

    [Test]
    public async Task LoadingRegressionTests_WithValidData_PopulatesSearchTable()
    {
      var testData = new List<RegressionTestDataObject>
      {
        new RegressionTestDataObject("Test1"),
        new RegressionTestDataObject("Test2")
      };

      SetupMockStrategy(new RegressionTestCollectionData()
      {
        Data = testData
      });

      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);

      Assert.That(mSut.SearchTableViewModel.SourceData, Has.Count.EqualTo(2));
      Assert.That(mSut.SearchTableViewModel.SourceData[0].RegressionTestData.TestName, Is.EqualTo("Test1"));
      Assert.That(mSut.SearchTableViewModel.SourceData[1].RegressionTestData.TestName, Is.EqualTo("Test2"));
    }

    [Test]
    public async Task LoadingRegressionTests_WithErrors_AddsErrorsToOutput()
    {
      var testData = new RegressionTestCollectionData()
      {
        Data = new List<RegressionTestDataObject>(),
        Errors =
        {
          new ErrorObject("Error1"),
          new ErrorObject("Error2")
        }
      };

      SetupMockStrategy(testData);

      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);

      var errorOutput = mSut.SearchTableViewModel.OutputControlViewModel.OutputList
        .Where(o => o.OutputType == OutputType.Error).ToList();

      Assert.That(errorOutput, Has.Count.EqualTo(2));
      Assert.That(errorOutput[0].Text, Is.EqualTo("Error1"));
      Assert.That(errorOutput[1].Text, Is.EqualTo("Error2"));
    }

    [Test]
    
    public async Task LoadingRegressionTests_WithSuccessfulResults_AddsSuccessMessageToOutput()
    {
      var testData = new RegressionTestCollectionData()
      {
        Data = [new RegressionTestDataObject("Test1"), new RegressionTestDataObject("Test2")]
      };

      SetupMockStrategy(testData);

      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);

      var successOutput = mSut.SearchTableViewModel.OutputControlViewModel.OutputList
        .Where(o => o.OutputType == OutputType.Success).ToList();

      Assert.That(successOutput, Has.Count.GreaterThan(0));
      Assert.That(successOutput.First().Text, Does.Contain("2"));
    }

    [Test]
    public async Task LoadRegressionTests_PassesPythonCommandToStrategy()
    {
      var mockStrategy = SetupMockStrategy(new RegressionTestCollectionData()
      {
        Data = new List<RegressionTestDataObject>()
      });

      mSut.MenuViewModel.SelectedPythonCommand = "python2";
      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);

      mockStrategy.Verify(x => x.Collect("python2"), Times.Once);
    }

    [Test]
    public async Task LoadingRegressionTests_ClearsPreviousDataBeforeAdding()
    {
      mSut.SearchTableViewModel.SourceData.Add(new RegressionTestViewModel(new RegressionTestDataObject("OldTest")));

      var testData = new RegressionTestCollectionData()
      {
        Data = [new RegressionTestDataObject("NewTest")]
      };

      SetupMockStrategy(testData);

      mSut.LoadRegressionTestsCommand.Execute(null);
      await Task.Delay(100);

      Assert.That(mSut.SearchTableViewModel.SourceData, Has.Count.EqualTo(1));
      Assert.That(mSut.SearchTableViewModel.SourceData[0].RegressionTestData.TestName, Is.EqualTo("NewTest"));
    }

    [Test]
    public void CancelLoadingCommand_WhenLoading_CanExecute()
    {
      mSut.IsLoadingData = true;
      Assert.That(mSut.CancelLoadingCommand.CanExecute(null), Is.True);
    }

    [Test]
    public async Task LoadRegressionTests_WhenStrategyThrows_HandlesGracefully()
    {
      var mockStrategy = new Mock<ICollectingStrategy>();
      mockStrategy.Setup(x => x.Collect(It.IsAny<string>()))
        .Throws(new FileNotFoundException("Test file not found"));
      mSut.CollectingStrategyHandler.ChangeCollectingStrategy(mockStrategy.Object);

      Assert.DoesNotThrowAsync(async () =>
      {
        mSut.LoadRegressionTestsCommand.Execute(null);
        await Task.Delay(500);
      });

      Assert.That(mSut.IsLoadingData, Is.False);
    }



    private Mock<ICollectingStrategy> SetupMockStrategy(RegressionTestCollectionData returnTestData)
    {
      var mockStrategy = new Mock<ICollectingStrategy>();
      mockStrategy.Setup(x => x.Collect(It.IsAny<string>()))
        .Returns(returnTestData);
      mSut.CollectingStrategyHandler.ChangeCollectingStrategy(mockStrategy.Object);
      return mockStrategy;
    }

    #endregion

    public class TestSettingsService : ISettingsService
    {
      public string SelectedTheme { get; set; } = "Light";
      public string SelectedStrategy { get; set; } = "CSharp";
      public string SelectedPythonCommand { get; set; } = "python";
      public bool IsUsingAbsolutePath { get; set; }
      public bool IsWindows { get; set; }
      public bool IsDebugSet { get; set; }
      public bool IsVerboseSet { get; set; }
      public bool IsExeIncluded { get; set; }
      public HighlighterSettings HighlighterSettings { get; } = new HighlighterSettings();
      public bool SaveCalled { get; private set; }
      public void Save() => SaveCalled = true;
    }



  }
}
