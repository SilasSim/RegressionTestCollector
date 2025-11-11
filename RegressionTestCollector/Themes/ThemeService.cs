using System.Windows;

namespace RegressionTestCollector.Themes
{
  public class ThemeService : IThemeService
  {
    /// <summary>
    /// Loads the Colors from Colors.<see cref="themeName"/>.xaml on current application.
    /// </summary>
    /// <param name="themeName"></param>
    public void ApplyTheme(string themeName)
    {
      var uri = new Uri($"Themes/Colors.{themeName}.xaml", UriKind.Relative);
      var newDict = new ResourceDictionary()
      {
        Source = uri
      };

      var appResources = Application.Current.Resources.MergedDictionaries;

      var oldDict = appResources.FirstOrDefault(d => d.Source.OriginalString.StartsWith("Themes/"));
      if (oldDict != null)
      {
        appResources.Remove(oldDict);
      }

      appResources.Insert(0, newDict);
    }
  }
}
