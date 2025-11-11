using System.Collections.ObjectModel;
using MVVMArchitecture;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.Models
{
  public class FilterSet : ObservableCollection<FilterElement>
  {
    public new void Add(FilterElement element)
    {
      if (this.All(item => item.Text != element.Text))
      {
        base.Add(element);
      }
    }
  }

  public class FilterElement : ObservableObject
  {
    public FilterElement(string text)
    {
      mText = text;
    }

    public FilterElement(string text, bool isChecked) : this(text)
    {
      IsChecked = isChecked;
    }

    private string mText;

    public string Text
    {
      get => mText;
      private set => SetField(ref mText, value);
    }

    private bool mIsChecked;

    public bool IsChecked
    {
      get => mIsChecked;
      set => SetField(ref mIsChecked, value);
    }
  }
}
