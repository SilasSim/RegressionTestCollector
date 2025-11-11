using System.Windows;

namespace RegressionTestCollector.Behaviors
{
    public static class Focuser
    {
        public static readonly DependencyProperty FocusTargetProperty = DependencyProperty.RegisterAttached(
          "FocusTarget", typeof(UIElement), typeof(Focuser), new PropertyMetadata(null, OnFocusTargetChanged));

        private static void OnFocusTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseDown += (sender, args) =>
                {
                    if (GetFocusTarget(element) is UIElement target)
                    {
                        target.Focus();
                        args.Handled = true;
                    }
                };
            }
        }

        public static void SetFocusTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(FocusTargetProperty, value);
        }

        public static UIElement GetFocusTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(FocusTargetProperty);
        }
    }
}
