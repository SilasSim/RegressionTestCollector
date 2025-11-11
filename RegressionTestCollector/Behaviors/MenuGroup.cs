using System.Windows;
using System.Windows.Controls;

namespace RegressionTestCollector.Behaviors
{
    public class MenuGroup : UIElement
    {
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.RegisterAttached(
          "GroupName", typeof(string), typeof(MenuGroup), new PropertyMetadata(OnGroupNameChanged));

        public static void SetGroupName(DependencyObject element, string value)
        {
            element.SetValue(GroupNameProperty, value);
        }

        public static string GetGroupName(DependencyObject element)
        {
            return (string)element.GetValue(GroupNameProperty);
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuItem item)
            {
                item.Click += MenuItem_Click;
            }
        }

        private static void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                var groupName = GetGroupName(item);
                if (!string.IsNullOrEmpty(groupName))
                {
                    UncheckOthersInGroup(item, groupName);
                    item.IsChecked = true;
                }
            }
        }

        private static void UncheckOthersInGroup(MenuItem menuItem, string groupName)
        {
            if (menuItem.Parent is MenuItem parent)
            {
                foreach (var item in parent.Items)
                {
                    if (item is MenuItem childItem)
                    {
                        childItem.IsChecked = false;
                    }
                }
            }
        }
    }
}
