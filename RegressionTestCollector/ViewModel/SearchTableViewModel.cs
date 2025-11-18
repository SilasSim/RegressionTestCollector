using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MVVMArchitecture;
using RegressionTestCollector.Models;
using RegressionTestCollector.Properties;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.ViewModel
{
  public class SearchTableViewModel : ViewModelBase
  {
    public OutputControlViewModel OutputControlViewModel { get; }
    public FilterControlViewModel GroupFilterViewModel { get; } 
    public ISettingsService SettingsService { get; }

    public Highlighter Highlighter { get; }

    public SearchTableViewModel(OutputControlViewModel outputControlViewModel, ISettingsService settingsService)
    {
      OutputControlViewModel = outputControlViewModel;
      SettingsService = settingsService;
      GroupFilterViewModel = new FilterControlViewModel(Resources.Strings.GroupNameFilterControl);
      CopyCommandStringCommand = new RelayCommand<RegressionTestViewModel>(CopyCommandString);

      PrepareForDebugSessionCommand = new RelayCommand<RegressionTestViewModel>(PrepareDebugSession);

      CleanUpDebugSessionCommand = new RelayCommand(CleanUpDebugSession);


      OpenFolderCommand = new RelayCommand<RegressionTestViewModel>(OpenFolder);
      mFilterTimer = new DispatcherTimer
      {
        Interval = mFilterTimerInterval
      };
      mFilterTimer.Tick += OnFilterTimerTick;
      SourceData = [];
      SourceData.CollectionChanged += OnSourceDataCollectionChanged;
      GroupFilterViewModel.FilterChanged += OnGroupFilterChanged;

      Highlighter = new Highlighter()
      {
        IsTurnedOff = SettingsService.HighlighterSettings.IsTurnedOff
      };

      Highlighter.PropertyValueChanged += OnHighlighterPropertyChange;
    }

    private void OnHighlighterPropertyChange(object? sender, PropertyValueChangedEventArgs e)
    {
      if (e.NewValue is bool newVal && e.PropertyName == nameof(Highlighter.IsTurnedOff))
      {
        SettingsService.HighlighterSettings.IsTurnedOff = newVal;
      }
    }


    private readonly List<DebugSessionInformation> mDebugSessionInformation = new ();

    internal void CleanUpDebugSession()
    {
      try
      {
        foreach (var info in mDebugSessionInformation)
        {
          File.Delete(info.Path);
          info.Test.IsDebug = false;
          OutputControlViewModel.OutputList.Add(new OutputData($"{info.Path} {Resources.CollectingErrors.DeletingDebugFile}"));
        }

        mDebugSessionInformation.Clear();

        if (mDebugSessionInformation.Count == 0)
        {
          CanDebugSessionBeCleanedUp = false;
        }

        OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.DebugCleanUpSuccess, OutputType.Success));
      }
      catch (Exception e)
      {
        OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.DebugCleanUpError, OutputType.Error));
      }
    }

    internal void PrepareDebugSession(RegressionTestViewModel? obj)
    {
      if (obj is not null && !String.IsNullOrWhiteSpace(obj.RegressionTestData.PythonScriptPath))
      {
        if (obj.IsDebug)
        {
          return;
        }

        var foundData = mDebugSessionInformation.Find(e =>
          e.Test.RegressionTestData.PythonScriptPath == obj.RegressionTestData.PythonScriptPath);

        if (foundData is not null)
        {
          var messageBoxResult = MessageBox.Show(Resources.SearchTableString.DebugWarningText, Resources.SearchTableString.DebugWarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);

          if (messageBoxResult == MessageBoxResult.Yes)
          {
            mDebugSessionInformation.Remove(foundData);
            foundData.Test.IsDebug = false;
          }
          else
          {
            return;
          }
        }

        var debugScriptPath = PythonHelper.CreateDebugCopy(obj.RegressionTestData);
        mDebugSessionInformation.Add(new DebugSessionInformation(debugScriptPath, obj));
        obj.IsDebug = true;
        OutputControlViewModel.OutputList.Add(new OutputData($"{debugScriptPath} {Resources.CollectingErrors.CreatedDebugScriptCopy}", OutputType.Success));
      }

      if (mDebugSessionInformation.Count > 0)
      {
        CanDebugSessionBeCleanedUp = true;
      }
      else
      {
        OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.DebugCreationError, OutputType.Error));
      }
    }


    internal void CopyCommandString(RegressionTestViewModel? obj)
    {
      if (obj is not null)
      {
        string copyString = String.Empty;
        try
        {
          copyString = StringUtils.CreateCommandString(obj, SettingsService.IsUsingAbsolutePath,
            SettingsService.IsExeIncluded, SettingsService.IsWindows, SettingsService.IsDebugSet,
            SettingsService.IsVerboseSet);
        }
        catch (ArgumentException e)
        {
          OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.ErrorCreatingCopyCommandString));
        }
        

        if (string.IsNullOrEmpty(copyString))
        {
          OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.ClipboardCopyError, OutputType.Error));
        }

        Clipboard.SetText(copyString);
      }
    }

    private static void OpenFolder(RegressionTestViewModel? obj)
    {
      if (obj != null)
      {
        Process.Start("explorer.exe", $"\"{obj.RegressionTestData.FolderPath}\"");
      }
    }


    private void OnGroupFilterChanged(object? sender, EventArgs e)
    {
      Filter();
    }


    /// <summary>
    /// Event for debouncing the filter operation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilterTimerTick(object? sender, EventArgs e)
    {
      mFilterTimer.Stop();
      Filter();
    }

    private readonly DispatcherTimer mFilterTimer;
    private readonly TimeSpan mFilterTimerInterval = TimeSpan.FromMilliseconds(200);

    public TimeSpan FilterTimerInterval
    {
      get => mFilterTimerInterval;
    }

    private string mSearchString = "";

    /// <summary>
    /// Is used for filtering the data in the table. Resets the filter timer, when changed.
    /// </summary>
    public string SearchString
    {
      get => mSearchString;
      set 
      {
        if (SetField(ref mSearchString, value))
        {
          mFilterTimer.Stop();
          mFilterTimer.Start();
        }
      }
    }

  
    /// <summary>
    /// Filtered view of the RegressionTestData which is displayed in the table
    /// </summary>
    public ICollectionView? FilteredData { get; private set; }
    private ObservableCollection<RegressionTestViewModel> mSourceData = [];

    /// <summary>
    /// The source data for the table. When set, it also resets the filter and search string and updates <see cref="FilteredData"/>.
    /// </summary>
    public ObservableCollection<RegressionTestViewModel> SourceData
    {
      get => mSourceData;
      set
      {
        if (SetField(ref mSourceData, value))
        {
          mSourceData.CollectionChanged += OnSourceDataCollectionChanged;
          FilteredData = CollectionViewSource.GetDefaultView(mSourceData);
          FilteredData.Filter = FilterPredicate;
          FilteredData.Refresh();
        }
      }
    }

    private void OnSourceDataCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      MatchCounter = new MatchCounter(mSourceData.Count, mSourceData.Count);
      UpdateGroupFilter();
    }

    private readonly Dictionary<string, string[]> mFilterChache = new();

    /// <summary>
    /// Predicate for filtering the data.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool FilterPredicate(object obj)
    {
      if (obj is RegressionTestViewModel item)
      {
        var groupFilterStrings = GroupFilterViewModel.GetActiveGroupFilter();
        if (string.IsNullOrEmpty(mSearchString) && groupFilterStrings.Count == 0)
        {
          return true;
        }

        bool searchStringFilterApplies = true;
        if (!string.IsNullOrEmpty(mSearchString))
        {
          string[] searchStrings = StringUtils.ParseSearchTerms(mSearchString, mFilterChache);
          searchStringFilterApplies = item.RegressionTestData.ContainsAny(searchStrings);
        }

        bool groupFilterApplies = groupFilterStrings.Count == 0;

        foreach (var groupString in groupFilterStrings)
        {
          if (item.RegressionTestData.TestGroup == groupString)
          {
            groupFilterApplies = true;
            break;
          }
        }
        return searchStringFilterApplies && groupFilterApplies;
      }

      return false;
    }

    /// <summary>
    /// Applies the filter to the <see cref="FilteredData"/>.
    /// </summary>
    internal void Filter()
    {

      FilteredData?.Refresh();

      MatchCounter = new MatchCounter(FilteredData?.Cast<object>().Count() ?? 0, SourceData.Count);
    }

    private MatchCounter mMatchCounter = new (0, 0);

    public MatchCounter MatchCounter
    {
      get => mMatchCounter;
      set => SetField(ref mMatchCounter, value);
    }

    public ICommand CopyCommandStringCommand { get; }
    public ICommand OpenFolderCommand { get; }

    public ICommand PrepareForDebugSessionCommand { get; }

    public ICommand CleanUpDebugSessionCommand { get; }

    private bool mCanDebugSessionBeCleanedUp;

    public bool CanDebugSessionBeCleanedUp
    {
      get => mCanDebugSessionBeCleanedUp;
      set => SetField(ref mCanDebugSessionBeCleanedUp, value);
    }

    internal void UpdateGroupFilter()
    {
      GroupFilterViewModel.FilterSet.Clear();
      var groups = SourceData.Select(elem => elem.RegressionTestData.TestGroup).Distinct().ToList();
      foreach (var groupName in groups)
      {
        GroupFilterViewModel.FilterSet.Add(new FilterElement(groupName));
      }

    }
  }
}
