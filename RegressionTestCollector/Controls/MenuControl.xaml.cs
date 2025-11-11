using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegressionTestCollector.Controls
{
  /// <summary>
  /// Interaction logic for MenuControl.xaml
  /// </summary>
  public partial class MenuControl : UserControl
  {
    public MenuControl()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty MenuDataProperty = DependencyProperty.Register(
      nameof(MenuData), typeof(string), typeof(MenuControl), new PropertyMetadata(default(string)));
      
    public string MenuData
    {
      get { return (string)GetValue(MenuDataProperty); }
      set { SetValue(MenuDataProperty, value); }
    }
  }
}
