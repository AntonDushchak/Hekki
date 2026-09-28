using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hekki.UI.Views.Shared.Behaviors
{
    public static class ShiftWheelScrolling
    {
        private const int LinesPerNotch = 3;

        public static void Register()
        {
            EventManager.RegisterClassHandler(
                typeof(ScrollViewer),
                UIElement.PreviewMouseWheelEvent,
                new MouseWheelEventHandler(OnPreviewMouseWheel));
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift) return;
            if (FindHorizontalScrollViewer(e.OriginalSource as DependencyObject) is not ScrollViewer scrollViewer) return;

            var notches = Math.Max(1, Math.Abs(e.Delta) / Mouse.MouseWheelDeltaForOneLine);
            for (var i = 0; i < notches * LinesPerNotch; i++)
            {
                if (e.Delta > 0)
                    scrollViewer.LineLeft();
                else
                    scrollViewer.LineRight();
            }

            e.Handled = true;
        }

        private static ScrollViewer? FindHorizontalScrollViewer(DependencyObject? element)
        {
            while (element != null)
            {
                if (element is ScrollViewer { ScrollableWidth: > 0 } scrollViewer)
                    return scrollViewer;

                element = element is Visual or System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(element)
                    : LogicalTreeHelper.GetParent(element);
            }
            return null;
        }
    }
}
