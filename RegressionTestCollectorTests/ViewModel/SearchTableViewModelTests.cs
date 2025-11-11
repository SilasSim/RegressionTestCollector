using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using Moq;
using NUnit.Framework;
using RegressionTestCollector.CollectingStrategies;
using RegressionTestCollector.Models;
using RegressionTestCollector.Resources;
using RegressionTestCollector.Themes;
using RegressionTestCollector.Utils;
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
    public void Constructor_InitializesChildVMs()
    {
      Assert.That(mSut.OutputControlViewModel, Is.Not.Null);
      Assert.That(mSut.GroupFilterViewModel, Is.Not.Null);
    }

    [Test]
    public void Constructor_InitializesGroupFilterViewModel()
    {
      Assert.That(mSut.GroupFilterViewModel.Name, Is.EqualTo("Group Filter"));
    }

    [Test]
    public void Constructor_InitializesEmptySourceData()
    {
      Assert.That(mSut.SourceData, Is.Empty);
    }

    [Test]
    public void Constructor_InitializesCommands()
    {
      Assert.That(mSut.CopyCommandStringCommand, Is.Not.Null);
      Assert.That(mSut.PrepareForDebugSessionCommand, Is.Not.Null);
      Assert.That(mSut.CleanUpDebugSessionCommand, Is.Not.Null);
      Assert.That(mSut.OpenFolderCommand, Is.Not.Null);
      Assert.That(mSut.CleanUpDebugSessionCommand, Is.Not.Null);
    }

    #endregion


    [Test]
    public void SourceData_WhenItemAdded_TriggersCollectionChanged()
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
    public void FilteredItems_WhenNoFilter_ShowsAllSourceData()
    {
      CreateAndSetTestData();

      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(mSut.SourceData.Count));
    }

    [Test]
    public void FilteredItems_WithTextFilter_ShowsMatchingItems()
    {
      CreateAndSetTestData();
      mSut.SearchString = "Command4";
      mSut.Filter();
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void FilteredItems_WithGroupFilter_ShowsOnlyMatchingGroups()
    {
      CreateAndSetTestData();
      mSut.GroupFilterViewModel.FilterSet[1].IsChecked = true;
      mSut.GroupFilterViewModel.UpdateFiltering();
      Assert.That(mSut.GroupFilterViewModel.GetActiveGroupFilter()[0], Is.EqualTo("Group2"));
      mSut.Filter();
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(2));
    }

    [Test]
    public void FilteredItems_WithCombinedFilters_ShowsCorrectSubset()
    {
      CreateAndSetTestData();
      mSut.GroupFilterViewModel.FilterSet[1].IsChecked = true;
      mSut.GroupFilterViewModel.UpdateFiltering();
      Assert.That(mSut.GroupFilterViewModel.GetActiveGroupFilter()[0], Is.EqualTo("Group2"));
      mSut.SearchString = "Command3";
      mSut.Filter();
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void FilteredItems_WhenSourceDataChanges_UpdatesAutomatically()
    {
      CreateAndSetTestData();
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(5));
      mSut.SourceData.Add(new RegressionTestViewModel(new RegressionTestDataObject("Test")));
      Assert.That(mSut.FilteredData.Cast<object>().Count(), Is.EqualTo(6));
    }

    #endregion

    [Test]
    public void CopyCommandString_EmptyCopyString_AddsErrorMessageToOutput()
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
