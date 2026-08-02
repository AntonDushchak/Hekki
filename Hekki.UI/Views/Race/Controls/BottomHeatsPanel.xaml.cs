using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class BottomHeatsPanel : UserControl
    {
        private Storyboard? _showStoryboard;
        private Storyboard? _hideStoryboard;

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(BottomHeatsPanel),
                new PropertyMetadata(false, OnIsExpandedChanged));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public BottomHeatsPanel()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _showStoryboard = (Storyboard)Resources["ShowPanelStoryboard"];
            _hideStoryboard = (Storyboard)Resources["HidePanelStoryboard"];

            if (!IsExpanded)
            {
                PanelTransform.Y = 80;
            }
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BottomHeatsPanel panel)
            {
                bool isExpanded = (bool)e.NewValue;

                if (isExpanded)
                {
                    panel.ShowPanel();
                }
                else
                {
                    panel.HidePanel();
                }
            }
        }

        private void Handle_Click(object sender, MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void ShowPanel()
        {
            _hideStoryboard?.Stop();
            _showStoryboard?.Begin();
        }

        private void HidePanel()
        {
            _showStoryboard?.Stop();
            _hideStoryboard?.Begin();
        }

        private void HeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HeatViewModel heatViewModel)
            {
                if (button.ContextMenu != null)
                {
                    button.ContextMenu.PlacementTarget = button;
                    button.ContextMenu.IsOpen = true;
                }
            }
        }
    }
}
