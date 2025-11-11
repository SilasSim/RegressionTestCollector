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
namespace RegressionTestCollector.Controls
{
  /// <summary>
  /// Interaction logic for SearchBox.xaml
  /// </summary>
  public partial class SearchBox : UserControl
  {
    public SearchBox()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty SearchStringProperty = DependencyProperty.Register(
      nameof(SearchString), typeof(string), typeof(SearchBox), new PropertyMetadata(default(string)));

    public string SearchString
    {
      get { return (string)GetValue(SearchStringProperty); }
      set { SetValue(SearchStringProperty, value); }
    }

    public static readonly DependencyProperty MatchCounterProperty = DependencyProperty.Register(
      nameof(MatchCounter), typeof(string), typeof(SearchBox), new PropertyMetadata("0 / 0"));

    public string MatchCounter
    {
      get { return (string)GetValue(MatchCounterProperty); }
      set { SetValue(MatchCounterProperty, value); }
    }
  }
}
