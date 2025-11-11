using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RegressionTestCollector.Models;
using RegressionTestCollector.Themes;
using RegressionTestCollector.ViewModel;

namespace RegressionTestCollector
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
  private ServiceProvider _serviceProvider;
    protected override void OnStartup(StartupEventArgs e)
    {

      var services = new ServiceCollection();

      services.AddSingleton<IThemeService, ThemeService>();
      services.AddSingleton<ISettingsService, SettingsService>();
      
      services.AddSingleton<OutputControlViewModel>();
      services.AddSingleton<SearchTableViewModel>();
      services.AddSingleton<MenuViewModel>();
      services.AddSingleton<MainViewModel>();

      services.AddSingleton<MainWindow>();

      _serviceProvider = services.BuildServiceProvider();

      base.OnStartup(e);
      
      var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
      var menuViewModel = _serviceProvider.GetRequiredService<MenuViewModel>();
      menuViewModel.InitializeTheme();
      mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      _serviceProvider?.Dispose();
      base.OnExit(e);
    }
  }

}
