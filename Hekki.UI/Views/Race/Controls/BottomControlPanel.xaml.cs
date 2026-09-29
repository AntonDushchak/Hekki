using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Hekki.UI.Converters;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class BottomControlPanel : UserControl
    {
        private const double HandleHeight = 40;
        private static readonly ColumnHeaderConverter HeaderConverter = new();
        private static readonly Duration AnimationDuration = TimeSpan.FromSeconds(0.3);

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(BottomControlPanel),
                new PropertyMetadata(false, OnIsExpandedChanged));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public BottomControlPanel()
        {
            InitializeComponent();
        }

        private double CollapsedOffset => Math.Max(0, PanelRoot.ActualHeight - HandleHeight);

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BottomControlPanel panel)
                panel.AnimateTo((bool)e.NewValue ? 0 : panel.CollapsedOffset, (bool)e.NewValue ? EasingMode.EaseOut : EasingMode.EaseIn);
        }

        private void PanelRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsExpanded) return;

            PanelTransform.BeginAnimation(TranslateTransform.YProperty, null);
            PanelTransform.Y = CollapsedOffset;
        }

        private void Handle_Click(object sender, MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void AnimateTo(double offset, EasingMode easingMode)
        {
            var animation = new DoubleAnimation(offset, AnimationDuration)
            {
                EasingFunction = new CubicEase { EasingMode = easingMode }
            };
            PanelTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void TableActionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || DataContext is not RaceViewModel viewModel) return;

            var totalTable = viewModel.TotalTable;
            var sortMenu = new MenuItem { Header = Localizer.Get("m_SortBy") };
            foreach (var column in totalTable.Columns.Where(c => c.IsSortable))
            {
                sortMenu.Items.Add(new MenuItem
                {
                    Header = HeaderConverter.Convert(column, typeof(string), null!, CultureInfo.CurrentCulture),
                    Command = totalTable.SortByColumnCommand,
                    CommandParameter = column
                });
            }

            var menu = new ContextMenu
            {
                PlacementTarget = button,
                Placement = PlacementMode.Top
            };
            menu.Items.Add(sortMenu);
            menu.Items.Add(new MenuItem
            {
                Header = Localizer.Get("m_ReverseOrder"),
                Command = totalTable.ReverseOrderCommand
            });
            menu.IsOpen = true;
        }

        private void HeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { ContextMenu: { } menu } button)
            {
                menu.PlacementTarget = button;
                menu.IsOpen = true;
            }
        }
    }
}
