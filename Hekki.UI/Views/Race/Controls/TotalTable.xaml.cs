using Hekki.UI.ViewModels;
using Hekki.UI.ViewModels.Race;
using Hekki.UI.ViewModels.Race.TotalTable;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class TotalTable : UserControl
    {
        public static readonly DependencyProperty ParticipantsProperty =
            DependencyProperty.Register(
                nameof(Participants),
                typeof(ParticipantsSectionViewModel),
                typeof(TotalTable));

        private const double DragHandleWidth = 24;
        private const string DragHandleTag = "DragHandle";

        private TotalTableViewModel? _viewModel;
        private TotalTableRowViewModel? _dragRow;
        private Point _dragStart;
        private InsertionAdorner? _insertionAdorner;

        public ParticipantsSectionViewModel? Participants
        {
            get => (ParticipantsSectionViewModel?)GetValue(ParticipantsProperty);
            set => SetValue(ParticipantsProperty, value);
        }

        public TotalTable()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.Columns.CollectionChanged -= OnColumnsChanged;

            _viewModel = e.NewValue as TotalTableViewModel;

            if (_viewModel != null)
            {
                _viewModel.Columns.CollectionChanged += OnColumnsChanged;
                RebuildHeader();
            }
        }

        private void OnColumnsChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => RebuildHeader();

        private void RebuildHeader()
        {
            PART_HeaderPanel.Children.Clear();
            if (_viewModel == null) return;

            var converter = Resources["ColumnHeaderConverter"] as IValueConverter;

            PART_HeaderPanel.Children.Add(new Border { Width = DragHandleWidth });

            foreach (var column in _viewModel.Columns)
            {
                var cell = new Grid();

                // Bind cell width to ColumnViewModel.ColumnWidth
                cell.SetBinding(WidthProperty, new Binding(nameof(ColumnViewModel.ColumnWidth))
                {
                    Source = column,
                    Mode = BindingMode.OneWay
                });

                // Header text
                var text = new TextBlock
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = column.IsNumeric ? TextAlignment.Center : TextAlignment.Left,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = column.IsNumeric ? new Thickness(0) : new Thickness(6, 0, 5, 0)
                };
                if (converter != null)
                    text.SetBinding(TextBlock.TextProperty, new Binding { Source = column, Converter = converter });
                else
                    text.Text = column.HeaderText ?? column.HeaderResourceKey ?? string.Empty;

                cell.Children.Add(text);

                // Resize thumb
                var thumb = new Thumb
                {
                    Width = 5,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Cursor = Cursors.SizeWE,
                    Template = CreateThumbTemplate()
                };
                var col = column;
                thumb.DragDelta += (_, e) =>
                    col.ColumnWidth = Math.Max(40, col.ColumnWidth + e.HorizontalChange);

                cell.Children.Add(thumb);

                PART_HeaderPanel.Children.Add(cell);
            }
        }

        private void Rows_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragRow = null;

            var source = e.OriginalSource as DependencyObject;
            if (FindAncestor<FrameworkElement>(source, el => DragHandleTag.Equals(el.Tag)) == null)
                return;

            _dragRow = FindAncestor<TotalTableRow>(source)?.DataContext as TotalTableRowViewModel;
            _dragStart = e.GetPosition(PART_Rows);
        }

        private void Rows_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragRow == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(PART_Rows);
            if (Math.Abs(position.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            var row = _dragRow;
            _dragRow = null;
            DragDrop.DoDragDrop(PART_Rows, new DataObject(typeof(TotalTableRowViewModel), row), DragDropEffects.Move);
            RemoveInsertionAdorner();
        }

        private void Rows_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TotalTableRowViewModel)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            e.Effects = DragDropEffects.Move;
            ShowInsertionAdorner(GetInsertionIndex(e.GetPosition(PART_Rows)));
            e.Handled = true;
        }

        private void Rows_DragLeave(object sender, DragEventArgs e)
        {
            RemoveInsertionAdorner();
        }

        private async void Rows_Drop(object sender, DragEventArgs e)
        {
            RemoveInsertionAdorner();

            if (_viewModel == null || e.Data.GetData(typeof(TotalTableRowViewModel)) is not TotalTableRowViewModel row)
                return;

            e.Handled = true;

            var oldIndex = _viewModel.TotalTableRows.IndexOf(row);
            var insertionIndex = GetInsertionIndex(e.GetPosition(PART_Rows));
            var newIndex = insertionIndex > oldIndex ? insertionIndex - 1 : insertionIndex;

            await _viewModel.MoveRowAsync(row, newIndex);
        }

        private int GetInsertionIndex(Point position)
        {
            var count = PART_Rows.Items.Count;
            for (var i = 0; i < count; i++)
            {
                if (PART_Rows.ItemContainerGenerator.ContainerFromIndex(i) is not FrameworkElement container)
                    continue;

                var top = container.TranslatePoint(new Point(0, 0), PART_Rows).Y;
                if (position.Y < top + container.ActualHeight / 2)
                    return i;
            }
            return count;
        }

        private double GetInsertionOffset(int insertionIndex)
        {
            var count = PART_Rows.Items.Count;
            if (count == 0) return 0;

            if (insertionIndex < count &&
                PART_Rows.ItemContainerGenerator.ContainerFromIndex(insertionIndex) is FrameworkElement container)
                return container.TranslatePoint(new Point(0, 0), PART_Rows).Y;

            if (PART_Rows.ItemContainerGenerator.ContainerFromIndex(count - 1) is FrameworkElement last)
                return last.TranslatePoint(new Point(0, last.ActualHeight), PART_Rows).Y;

            return 0;
        }

        private void ShowInsertionAdorner(int insertionIndex)
        {
            if (_insertionAdorner == null)
            {
                var layer = AdornerLayer.GetAdornerLayer(PART_Rows);
                if (layer == null) return;

                var brush = TryFindResource("Brush.ActionDark") as Brush ?? Brushes.DodgerBlue;
                _insertionAdorner = new InsertionAdorner(PART_Rows, brush);
                layer.Add(_insertionAdorner);
            }

            _insertionAdorner.OffsetY = GetInsertionOffset(insertionIndex);
        }

        private void RemoveInsertionAdorner()
        {
            if (_insertionAdorner == null) return;

            AdornerLayer.GetAdornerLayer(PART_Rows)?.Remove(_insertionAdorner);
            _insertionAdorner = null;
        }

        private static T? FindAncestor<T>(DependencyObject? current, Func<T, bool>? predicate = null) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T match && (predicate == null || predicate(match)))
                    return match;

                current = current is Visual or System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(current)
                    : LogicalTreeHelper.GetParent(current);
            }
            return null;
        }

        private static ControlTemplate CreateThumbTemplate()
        {
            var template = new ControlTemplate(typeof(Thumb));
            var rect = new FrameworkElementFactory(typeof(Rectangle));
            rect.SetValue(Rectangle.FillProperty, new SolidColorBrush(Colors.Transparent));
            rect.SetValue(CursorProperty, Cursors.SizeWE);
            template.VisualTree = rect;
            return template;
        }
    }
}
