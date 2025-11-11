using System.Windows;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollector
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow(MainViewModel viewModel)
    {

      InitializeComponent();
      DataContext = viewModel;

      // This is a fix for the search table not using the rest of the window as height, when the output control is closed
      if (DataContext is MainViewModel vm)
      {
        vm.SearchTableViewModel.OutputControlViewModel.VisibilityChanged += (sender, obj) =>
        {
          if (!vm.SearchTableViewModel.OutputControlViewModel.IsVisible)
          {
            GridSplitterRowDefinition.Height = new GridLength(0, 0);
            OutputControlRowDefinition.Height = new GridLength(0, 0);

            SearchTableRowDefinition.Height = new GridLength(1, GridUnitType.Star);
          }
          else
          {
            GridSplitterRowDefinition.Height = new GridLength(5, GridUnitType.Pixel);
            OutputControlRowDefinition.Height = new GridLength(200, GridUnitType.Pixel);
          }
        };
      }
    }
  }
}