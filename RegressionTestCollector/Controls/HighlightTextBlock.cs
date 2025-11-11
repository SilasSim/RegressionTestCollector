using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.Controls
{
  /// <summary>
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:RegressionTestCollector.Controls"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:RegressionTestCollector.Controls;assembly=RegressionTestCollector.Controls"
  ///
  /// You will also need to add a project reference from the project where the XAML file lives
  /// to this project and Rebuild to avoid compilation errors:
  ///
  ///     Right click on the target project in the Solution Explorer and
  ///     "Add Reference"->"Projects"->[Browse to and select this project]
  ///
  ///
  /// Step 2)
  /// Go ahead and use your control in the XAML file.
  ///
  ///     <MyNamespace:CustomControl1/>
  ///
  /// </summary>
  public class HighlightTextBlock : TextBlock
  {
    public HighlightTextBlock()
    {
      this.UpdateInlines();
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text), typeof(string), typeof(HighlightTextBlock),
      new PropertyMetadata(default(string), OnHighlightChanged));

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register(
      nameof(Highlight), typeof(string), typeof(HighlightTextBlock),
      new PropertyMetadata(default(string), OnHighlightChanged));

    public string Highlight
    {
      get { return (string)GetValue(HighlightProperty); }
      set { SetValue(HighlightProperty, value); }
    }

    public static readonly DependencyProperty HighlightTurnedOffProperty = DependencyProperty.Register(
      nameof(HighlightTurnedOff), typeof(bool), typeof(HighlightTextBlock),
      new PropertyMetadata(default(bool), OnHighlightChanged));

    public bool HighlightTurnedOff
    {
      get { return (bool)GetValue(HighlightTurnedOffProperty); }
      set { SetValue(HighlightTurnedOffProperty, value); }
    }

    private static void OnHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is HighlightTextBlock block)
      {
        block.UpdateInlines();
      }
    }

    private static readonly Dictionary<string, string[]> mHightlightCache = new ();
    private void UpdateInlines()
    {
      this.Inlines.Clear();

      if (HighlightTurnedOff || string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(Highlight))
      {
        this.Inlines.Add(new Run(Text ?? ""));
        return;
      }

      var highlights = StringUtils.ParseSearchTerms(Highlight, mHightlightCache);
      var pattern = string.Join("|", highlights.Select(h => Regex.Escape(h)));

      if (String.IsNullOrEmpty(pattern))
      {
        this.Inlines.Add(new Run(Text));
        return;
      }

      var regex = new Regex(pattern, RegexOptions.IgnoreCase);
      var matches = regex.Matches(Text);

      int lastIndex = 0;
      foreach (Match match in matches)
      {
        if (match.Index > lastIndex)
        {
          this.Inlines.Add(new Run(Text.Substring(lastIndex, match.Index - lastIndex)));
        }

        this.Inlines.Add(new Run(match.Value)
        {
          Foreground = TryFindResource("Brush.HighlightedText") as Brush ?? Brushes.Black,
          Background = TryFindResource("Brush.HighlightSearch") as Brush ?? Brushes.Yellow,
        });

        lastIndex = match.Index + match.Length;

      }

      if (lastIndex < Text.Length)
      {
        this.Inlines.Add(new Run(Text.Substring(lastIndex)));
      }

    }
  }

}
