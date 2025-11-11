using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using MVVMArchitecture;
using RegressionTestCollector.Models;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.ViewModel
{
  public class FilterControlViewModel : ViewModelBase
  {
    public event EventHandler? FilterChanged;

    public FilterControlViewModel(string name)
    {
      mName = name;
      FilterSet = [];
      FilterGroupCommand = new RelayCommand<FilterElement>(OnIsCheckedChanged);
      FilterAllCommand = new RelayCommand(OnAllIsCheckedChanged);
      UnselectAll = new RelayCommand(OnUnselectAll);
    }

    private void OnUnselectAll()
    {
      FilterAllElement.IsChecked = false;
      OnAllIsCheckedChanged();
    }

    private List<string> mActiveGroupFilter = [];

    public IReadOnlyList<string> GetActiveGroupFilter()
    {
      return mActiveGroupFilter;
    }

    private void OnAllIsCheckedChanged()
    {
      foreach (var filterElement in FilterSet)
      {
        filterElement.IsChecked = FilterAllElement.IsChecked;
      }
      UpdateFiltering();
    }

    public RelayCommand FilterAllCommand { get; }

    private void OnIsCheckedChanged(FilterElement? obj)
    {
      if (obj == null)
      {
        return;
      }

      UpdateFiltering();
    }

    internal void UpdateFiltering()
    {
      mActiveGroupFilter = FilterSet
        .Where(elem => elem.IsChecked)
        .Select(elem => elem.Text).ToList();
      FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    private FilterElement mFilterAllElement = new("(All)");

    public FilterElement FilterAllElement
    {
      get => mFilterAllElement;
      set
      {
        if (SetField(ref mFilterAllElement, value))
        {
          foreach (var filterElement in FilterSet)
          {
            filterElement.IsChecked = FilterAllElement.IsChecked;
          }
        }
      }
    }

    private FilterSet mFilterSet = [];

    public FilterSet FilterSet
    {
      get => mFilterSet;
      set
      {
        if (SetField(ref mFilterSet, value))
        {
          UpdateFilterSetView();
        }
      }
    }

    private ICollectionView? mFilterSetView;

    public ICollectionView? FilterSetView
    {
      get => mFilterSetView;
      private set => SetField(ref mFilterSetView, value);
    }

    private void UpdateFilterSetView()
    {
      if (FilterSet != null)
      {
        FilterSetView = CollectionViewSource.GetDefaultView(FilterSet);
        FilterSetView.SortDescriptions.Add(new SortDescription("Text", ListSortDirection.Ascending));
        FilterSetView.Filter = FilterPredicate;
        FilterSetView.Refresh();
      }
      else
      {
        FilterSetView = null;
      }
    }

    private bool FilterPredicate(object obj)
    {
      if (obj is FilterElement item)
      {
        if (string.IsNullOrEmpty(mSearchString))
        {
          return true;
        }
        string[] searchStrings = mSearchString.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return StringUtils.StringContainsAllStrings(item.Text, searchStrings);
      }

      return false;
    }

    private string mName;

    public string Name
    {
      get => mName;
      set => SetField(ref mName, value);
    }

    private string mSearchString = "";

    public string SearchString
    {
      get => mSearchString;
      set
      {
        if (SetField(ref mSearchString, value))
        {
          FilterSetView?.Refresh();
          MatchCounter.TotalCounter = FilterSet.Count;
          MatchCounter.VisibleCounter = FilterSetView?.Cast<object>()?.Count() ?? 0;
        }
      }
    }

    private MatchCounter mMatchCounter = new(0, 0);

    public MatchCounter MatchCounter
    {
      get => mMatchCounter;
      set => SetField(ref mMatchCounter, value);
    }

    public ICommand FilterGroupCommand { get; }
    public ICommand UnselectAll { get; }
  }
}
