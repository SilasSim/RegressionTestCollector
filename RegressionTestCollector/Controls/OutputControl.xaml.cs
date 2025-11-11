using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollector.Controls
{
  /// <summary>
  /// Interaction logic for OutputControl.xaml
  /// </summary>
  public partial class OutputControl : UserControl
  {
    public OutputControl()
    {
      InitializeComponent();

      DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is OutputControlViewModel oldVm)
      {
        oldVm.OutputList.CollectionChanged -= OnCollectionChanged;
      }

      if (e.NewValue is OutputControlViewModel newVm)
      {
        newVm.OutputList.CollectionChanged += OnCollectionChanged;
      }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        Dispatcher.BeginInvoke(() =>
        {
          OutputListView.ScrollIntoView(OutputListView.Items[^1]);
        });

      }
    }
  }
}
