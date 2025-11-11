using System.Collections.ObjectModel;
using System.Windows.Input;
using MVVMArchitecture;
using RegressionTestCollector.Models;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.ViewModel
{
  public class OutputControlViewModel : ViewModelBase
  {
    public event EventHandler? VisibilityChanged;
    public OutputControlViewModel()
    {
      ToggleVisibilityCommand = new RelayCommand(ToggleVisibility);
      ClearListCommand = new RelayCommand(ClearList);
      OutputList = [];
    }

    private void ClearList()
    {
      OutputList.Clear();
    }

    private void ToggleVisibility()
    {
      IsVisible = !IsVisible;
    }

    public ObservableCollection<OutputData> OutputList { get; private set; }

    private bool mIsVisible = false;

    public bool IsVisible
    {
      get => mIsVisible;
      set
      {
        if (SetField(ref mIsVisible, value))
        {
          VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public ICommand ToggleVisibilityCommand { get; }
    public ICommand ClearListCommand { get; }
  }
}
