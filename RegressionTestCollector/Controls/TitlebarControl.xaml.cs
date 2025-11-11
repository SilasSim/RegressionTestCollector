using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RegressionTestCollector.ViewModel;
using RegressionTestCollector.Resources;

namespace RegressionTestCollector.Controls
{
  /// <summary>
  /// Interaction logic for TitlebarControl.xaml
  /// </summary>
  public partial class TitlebarControl : UserControl
  {
    public TitlebarControl()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty HostWindowProperty = DependencyProperty.Register(
      nameof(HostWindow), typeof(Window), typeof(TitlebarControl), new PropertyMetadata(OnWindowChanged));

    private static void OnWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is TitlebarControl titlebarControl)
      {
        titlebarControl.TitleText.Text = titlebarControl.HostWindow.Title;
      }
    }

    public Window HostWindow
    {
      get { return (Window)GetValue(HostWindowProperty); }
      set { SetValue(HostWindowProperty, value); }
    }

    public static readonly DependencyProperty SelectedThemeProperty = DependencyProperty.Register(
      nameof(SelectedTheme), typeof(string), typeof(TitlebarControl), new PropertyMetadata(default(string)));

    public string SelectedTheme
    {
      get { return (string)GetValue(SelectedThemeProperty); }
      set { SetValue(SelectedThemeProperty, value); }
    }


    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
      if (HostWindow.DataContext is MainViewModel vm)
      {
        if (vm.SearchTableViewModel.CanDebugSessionBeCleanedUp)
        {
          var warningResult = MessageBox.Show(RegressionTestCollector.Resources.Strings.DebugWarningText,
            RegressionTestCollector.Resources.Strings.DebugWarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
          if (warningResult == MessageBoxResult.Yes)
          {
            vm.SearchTableViewModel.CleanUpDebugSessionCommand.Execute(null);
          }
          else
          {
            return;
          }
        }
      }
      HostWindow.Close();
    }

    private void ButtonMaximize_OnClick(object sender, RoutedEventArgs e)
    {
      if (HostWindow.WindowState == WindowState.Maximized)
      {
        HostWindow.WindowState = WindowState.Normal;
      }
      else
      {
        HostWindow.WindowState = WindowState.Maximized;
      }
    }

    private void ButtonMinimize_OnClick(object sender, RoutedEventArgs e)
    {
      HostWindow.WindowState = WindowState.Minimized;
    }

    private void TitlebarControl_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      HostWindow.DragMove();
    }

    private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount >= 2)
      {
        e.Handled = true;
        ButtonClose_OnClick(sender, e);
      }
    }
  }
}
