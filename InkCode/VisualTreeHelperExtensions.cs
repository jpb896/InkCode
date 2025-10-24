using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace InkCode
{
    public static class VisualTreeHelperExtensions
    {
        public static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T parentAsT)
                {
                    return parentAsT;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
