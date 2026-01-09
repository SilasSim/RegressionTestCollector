using System.Collections.Specialized;
using RegressionTestCollector.Models;
using RegressionTestCollector.ViewModel;
using RegressionTestCollectorTests.TestUtils;

namespace RegressionTestCollectorTests.ViewModel
{
  [TestFixture]
  [Apartment(ApartmentState.STA)]
  public class SearchTableViewModelTests
  {
    private SearchTableViewModel mSut;
    private TestSettingsService mTestSettingsService;
    private OutputControlViewModel mOutputControlViewModel;

    [SetUp]
    public void SetUp()
    {
      mTestSettingsService = new TestSettingsService();
      mOutputControlViewModel = new OutputControlViewModel();
      mSut = new SearchTableViewModel(mOutputControlViewModel, mTestSettingsService);
    }

    #region Constructor Tests

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesChildViewModels()
    {
      Assert.That(mSut.OutputControlViewModel, Is.Not.Null);
      Assert.That(mSut.GroupFilterViewModel, Is.Not.Null);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesGroupFilterViewModel()
    {
      Assert.That(mSut.GroupFilterViewModel.Name, Is.EqualTo("Group Filter"));
    }

    [Test]
    public void WhenConstructorIsCalled_ThenSourceDataIsEmpty()
    {
      Assert.That(mSut.SourceData, Is.Empty);
    }

    [Test]
    public void WhenConstructorIsCalled_ThenInitializesAllCommands()
    {
      Assert.That(mSut.CopyCommandStringCommand, Is.Not.Null);
      Assert.That(mSut.PrepareForDebugSessionCommand, Is.Not.Null);
      Assert.That(mSut.CleanUpDebugSessionCommand, Is.Not.Null);
      Assert.That(mSut.OpenFolderCommand, Is.Not.Null);
      Assert.That(mSut.CleanUpDebugSessionCommand, Is.Not.Null);
    }

    #endregion


    [Test]
    public void GivenSourceData_WhenItemIsAdded_ThenTriggersCollectionChangedEvent()
    {
      bool eventFired = false;
      NotifyCollectionChangedEventArgs? eventArgs = null;

      mSut.SourceData.CollectionChanged += (sender, e) =>
      {
        eventFired = true;
        eventArgs = e;
      };
      var testItem = new RegressionTestViewModel(new RegressionTestDataObject("Test1"));

      mSut.SourceData.Add(testItem);
      Assert.That(eventFired, Is.True);
      Assert.That(eventArgs?.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
      Assert.That(eventArgs?.NewItems?[0], Is.EqualTo(testItem));
    }

    #region FilterItems Tests

    [Test]
    public void GivenNoFilter_WhenFilterIsApplied_ThenShowsAllSourceData()
    {
      CreateAndSetTestData();

      Assert.That(mSut?.FilteredData?.Cast<object>().Count(), Is.EqualTo(mSut?.SourceData.Count));
    }

    [Test]
    public void GivenTextFilter_WhenFilterIsApplied_ThenShowsMatchingItems()
    {
      CreateAndSetTestData();
      mSut.SearchString = "Command4";
      mSut.Filter();
      Assert.That(mSut?.FilteredData?.Cast<object>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void GivenGroupFilter_WhenFilterIsApplied_ThenShowsOnlyMatchingGroups()
    {
      CreateAndSetTestData();
      mSut.GroupFilterViewModel.FilterSet[1].IsChecked = true;
      mSut.GroupFilterViewModel.UpdateFiltering();
      Assert.That(mSut.GroupFilterViewModel.GetActiveGroupFilter()[0], Is.EqualTo("Group2"));
      mSut.Filter();
      Assert.That(mSut?.FilteredData?.Cast<object>().Count(), Is.EqualTo(2));
    }

    [Test]
    public void GivenTextAndGroupFilter_WhenFilterIsApplied_ThenShowsCorrectSubset()
    {
      CreateAndSetTestData();
      mSut.GroupFilterViewModel.FilterSet[1].IsChecked = true;
      mSut.GroupFilterViewModel.UpdateFiltering();
      Assert.That(mSut.GroupFilterViewModel.GetActiveGroupFilter()[0], Is.EqualTo("Group2"));
      mSut.SearchString = "Command3";
      mSut.Filter();
      Assert.That(mSut?.FilteredData?.Cast<object>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void GivenSourceDataChanges_WhenFilterIsApplied_ThenUpdatesFilteredItemsAutomatically()
    {
      CreateAndSetTestData();
      Assert.That(mSut?.FilteredData?.Cast<object>().Count(), Is.EqualTo(5));
      mSut.SourceData.Add(new RegressionTestViewModel(new RegressionTestDataObject("Test")));
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(6));
    }

    #endregion

    [Test]
    public void GivenEmptyCommandString_WhenCopyCommandStringIsCalled_ThenAddsErrorMessageToOutput()
    {
      mSut.CopyCommandString(new RegressionTestViewModel(new RegressionTestDataObject("Test1", folderPath:"Test")));
      Assert.That(mSut.OutputControlViewModel.OutputList, Has.Count.GreaterThan(1));
      Assert.That(mSut.OutputControlViewModel.OutputList[0].Text, Does.Contain("Exception"));
    }

    private void CreateAndSetTestData()
    {
      var data = new List<RegressionTestViewModel>
      {
        new(new RegressionTestDataObject(testName: "Test1", testGroup: "Group1", testCommand: "Command1",
          folderPath: "FolderPath1")),
        new(new RegressionTestDataObject(testName: "Test2", testGroup: "Group2", testCommand: "Command2",
          folderPath: "FolderPath1")),
        new(new RegressionTestDataObject(testName: "Test3", testGroup: "Group2", testCommand: "Command3",
          folderPath: "FolderPath2")),
        new(new RegressionTestDataObject(testName: "Test4", testGroup: "Group3", testCommand: "Command4",
          folderPath: "FolderPath2")),
        new(new RegressionTestDataObject(testName: "Test5", testGroup: "Group3", testCommand: "Command3",
          folderPath: "FolderPath2")),
      };

      foreach (var regressionTestViewModel in data)
      {
        mSut.SourceData.Add(regressionTestViewModel);
      }
    }

  }
}
