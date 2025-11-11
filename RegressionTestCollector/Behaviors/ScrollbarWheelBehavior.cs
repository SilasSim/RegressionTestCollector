using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RegressionTestCollector.Behaviors
{
  public class ScrollbarWheelBehavior
  {
    public static readonly DependencyProperty EnableHorizontalScrollProperty = DependencyProperty.RegisterAttached(
      "EnableHorizontalScroll", typeof(bool), typeof(ScrollbarWheelBehavior), new PropertyMetadata(false, OnChanged));

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is FrameworkElement frameworkElement)
      {
        if ((bool)e.NewValue)
        {
          frameworkElement.Loaded += OnLoaded;
          frameworkElement.Unloaded += OnUnloaded;
        }
        else
        {
          frameworkElement.Loaded -= OnLoaded;
          frameworkElement.Unloaded -= OnUnloaded;
          if (FindScrollViewer(frameworkElement) is ScrollViewer scrollViewer)
          {
            scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
          }
        }
      }
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (sender is not DependencyObject dependencyObject)
      {
        return;
      }
      var scrollViewer = FindScrollViewer(dependencyObject);
      if (scrollViewer != null)
      {
        Attach(scrollViewer);
      }

    }

    private static void Attach(ScrollViewer scrollViewer)
    {
      scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
      scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
    }



    private static void OnUnloaded(object? sender, RoutedEventArgs e)
    {
      if (sender is not DependencyObject dependencyObject) { return;}

      var scrollViewer = FindScrollViewer(dependencyObject);
      if (scrollViewer != null)
      {
        scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
      }
    }

    private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (sender is not ScrollViewer scrollViewer) { return;}

      

      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
      {
        if (e.Delta > 0)
        {
          scrollViewer?.LineLeft();
        }
        else
        {
          scrollViewer?.LineRight();
        }
        e.Handled = true;
      }
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject obj)
    {
      if (obj is ScrollViewer scrollViewer)
      {
        return scrollViewer;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        var found = FindScrollViewer(child);
        if (found != null) return found;
      }

      return null;
    }

    public static void SetEnableHorizontalScroll(DependencyObject element, bool value)
    {
      element.SetValue(EnableHorizontalScrollProperty, value);
    }

    public static bool GetEnableHorizontalScroll(DependencyObject element)
    {
      return (bool)element.GetValue(EnableHorizontalScrollProperty);
    }
  }
}
