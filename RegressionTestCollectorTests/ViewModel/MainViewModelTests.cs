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
    public void WhenConstructorIsCalled_ThenInitializesChildViewModels()
    {
      Assert.That(mSut.SearchTableViewModel, Is.Not.Null);
      Assert.That(mSut.MenuViewModel, Is.Not.Null);
      Assert.That(mSut.SearchTableViewModel.OutputControlViewModel, Is.Not.Null);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesAllCommands()
    {
      Assert.That(mSut.CancelLoadingCommand, Is.Not.Null);
      Assert.That(mSut.ChangeCollectingStrategyCommand, Is.Not.Null);
      Assert.That(mSut.ChangePythonCommandCommand, Is.Not.Null);
      Assert.That(mSut.ChangeThemeCommand, Is.Not.Null);
      Assert.That(mSut.LoadRegressionTestsCommand, Is.Not.Null);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesStrategyHandler()
    {
      Assert.That(mSut.CollectingStrategyHandler, Is.Not.Null);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.Not.Null);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesLoadingProgress()
    {
      Assert.That(mSut.LoadingProgress, Is.Not.Null);
      Assert.That(mSut.LoadingProgress.Current, Is.Zero);
    }

    #endregion

    #region Property Tests


    [Test]
    public void WhenLoadingDataTextIsSet_ThenValueBeRetrieved()
    {
      mSut.LoadingDataText = "Test text";
      Assert.That(mSut.LoadingDataText, Is.EqualTo("Test text"));
    }

    [Test]
    public void WhenConstructorIsCalled_ThenLoadingDataTextHasDefaultValue()
    {
      Assert.That(mSut.LoadingDataText, Is.Not.Null);
      Assert.That(mSut.LoadingDataText, Is.Not.Empty);
    }

    #endregion

    #region Command Tests

    [Test]
    public void GivenValidTheme_WhenChangeThemeCommandIsExecuted_ThenUpdatesMenuViewModelSelectedTheme()
    {
      var newTheme = "Some Theme";
      mSut.ChangeThemeCommand.Execute(newTheme);
      Assert.That(mSut.MenuViewModel.SelectedTheme, Is.EqualTo(newTheme));
    }

    [Test]
    public void GivenInvalidTheme_WhenChangeThemeCommandIsExecuted_ThenDoesNothing()
    {
      var initTheme = mSut.MenuViewModel.SelectedTheme;
      mSut.ChangeThemeCommand.Execute(null);
      Assert.That(mSut.MenuViewModel.SelectedTheme, Is.EqualTo(initTheme));
    }

    [Test]
    public void GivenValidPythonCommand_WhenChangePythonCommandCommandIsExecuted_ThenUpdatesMenuViewModelSelectedPythonCommand()
    {
      var newPythonCommand = "python2";
      mSut.ChangePythonCommandCommand.Execute(newPythonCommand);
      Assert.That(mSut.MenuViewModel.SelectedPythonCommand, Is.EqualTo(newPythonCommand));
    }

    [Test]
    public void GivenInvalidPythonCommand_WhenChangePythonCommandCommandIsExecuted_ThenDoesNothing()
    {
      var initPythonCommand = mSut.MenuViewModel.SelectedPythonCommand;
      mSut.ChangePythonCommandCommand.Execute(null);
      Assert.That(mSut.MenuViewModel.SelectedPythonCommand, Is.EqualTo(initPythonCommand));
    }

    #endregion

    #region Strategy Change Tests

    [Test]
    public void GivenCSharpStrategyName_WhenChangeCollectingStrategyCommandIsExecuted_ThenSetsCSharpStrategy()
    {
      var cSharpName = "C#";
      mSut.ChangeCollectingStrategyCommand.Execute(cSharpName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.TypeOf<CSharpCollectingStrategy>());
      Assert.That(mSut.MenuViewModel.SelectedStrategy, Is.EqualTo(cSharpName));
    }


    [Test]
    public void GivenPythonStrategyName_WhenChangeCollectingStrategyCommandIsExecuted_ThenSetsPythonStrategy()
    {
      var pythonName = "Python";
      mSut.ChangeCollectingStrategyCommand.Execute(pythonName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.TypeOf<PythonCollectingStrategy>());
      Assert.That(mSut.MenuViewModel.SelectedStrategy, Is.EqualTo(pythonName));
    }


    [Test]
    public void GivenUnknownStrategyName_WhenChangeCollectingStrategyCommandIsExecuted_ThenDoesNothing()
    {
      var originalStrategy = mSut.CollectingStrategyHandler.CollectingStrategy;

      var unknownName = "RandomTestStrategy";
      mSut.ChangeCollectingStrategyCommand.Execute(unknownName);
      Assert.That(mSut.CollectingStrategyHandler.CollectingStrategy, Is.EqualTo(originalStrategy));
    }

    #endregion

    #region PropertyChanged Tests

    [Test]
    public void WhenLoadingDataTextChanges_ThenTriggersPropertyChangedNotification()
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
    public void WhenIsLoadingDataIsChanged_ThenTriggersPropertyChangedNotification()
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
    public void WhenConstructorIsCalled_ThenLoadRegressionTestsCommandCanExecute()
    {
      Assert.That(mSut.LoadRegressionTestsCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void GivenLoadRegressionTestsCommand_WhenExecutionStarts_ThenSetsIsLoadingDataTrue()
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
    public async Task GivenLoadRegressionTestsCommand_WhenExecutionCompletes_ThenSetsIsLoadingDataFalse()
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
    public async Task GivenValidRegressionTestData_WhenLoadRegressionTestsCommandCompletes_ThenPopulatesSearchTable()
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
    public async Task GivenRegressionTestDataWithErrors_WhenLoadRegressionTestsCommandCompletes_ThenAddsErrorsToOutput()
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
    
    public async Task GivenSuccessfulRegressionTestData_WhenLoadRegressionTestsCommandCompletes_ThenAddsSuccessMessageToOutput()
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
    public async Task GivenSelectedPythonCommand_WhenLoadRegressionTestsCommandExecutes_ThenPassesPythonCommandToStrategy()
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
    public async Task GivenExistingSourceData_WhenLoadRegressionTestsCommandExecutes_ThenClearsPreviousDataBeforeAddingNew()
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
    public void WhenIsLoadingDataIsSetToTrue_ThenCancelLoadingCommandCanExecute()
    {
      mSut.IsLoadingData = true;
      Assert.That(mSut.CancelLoadingCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void WhenIsLoadingDataIsSetToTrue_ThenCancelLoadingCommandCannotExecute()
    {
      mSut.IsLoadingData = false;
      Assert.That(mSut.CancelLoadingCommand.CanExecute(null), Is.False);
    }

    [Test]
    public async Task GivenCollectingStrategyThrowsException_WhenLoadRegressionTestsCommandExecutes_ThenHandlesGracefullyAndStopsLoading()
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
