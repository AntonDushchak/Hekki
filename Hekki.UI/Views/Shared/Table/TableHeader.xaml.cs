using Hekki.UI.Converters;
using Hekki.UI.ViewModels;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hekki.UI.Views.Shared.Table
{
    public partial class TableHeader : UserControl
    {
        private const double MinColumnWidth = 40;
        private static readonly ColumnHeaderConverter HeaderConverter = new();

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(IEnumerable),
                typeof(TableHeader),
                new PropertyMetadata(null, OnColumnsChanged));

        public static readonly DependencyProperty LeadingWidthProperty =
            DependencyProperty.Register(
                nameof(LeadingWidth),
                typeof(double),
                typeof(TableHeader),
                new PropertyMetadata(0d, (d, _) => ((TableHeader)d).Rebuild()));

        public IEnumerable? Columns
        {
            get => (IEnumerable?)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public double LeadingWidth
        {
            get => (double)GetValue(LeadingWidthProperty);
            set => SetValue(LeadingWidthProperty, value);
        }

        public TableHeader()
        {
            InitializeComponent();
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var header = (TableHeader)d;

            if (e.OldValue is INotifyCollectionChanged oldColumns)
                oldColumns.CollectionChanged -= header.OnColumnsCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newColumns)
                newColumns.CollectionChanged += header.OnColumnsCollectionChanged;

            header.Rebuild();
        }

        private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

        private void Rebuild()
        {
            PART_Panel.Children.Clear();
            if (Columns == null) return;

            if (LeadingWidth > 0)
                PART_Panel.Children.Add(new Border { Width = LeadingWidth });

            foreach (var column in Columns.OfType<TableColumnViewModel>())
                PART_Panel.Children.Add(CreateHeaderCell(column));
        }

        private static Grid CreateHeaderCell(TableColumnViewModel column)
        {
            var cell = new Grid();
            cell.SetBinding(WidthProperty, new Binding(nameof(TableColumnViewModel.ColumnWidth))
            {
                Source = column,
                Mode = BindingMode.OneWay
            });

            var text = new TextBlock
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = column.IsNumeric ? TextAlignment.Center : TextAlignment.Left,
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = column.IsNumeric ? new Thickness(0) : new Thickness(6, 0, 5, 0)
            };
            text.SetBinding(TextBlock.TextProperty, new Binding { Source = column, Converter = HeaderConverter });
            cell.Children.Add(text);

            var thumb = new Thumb
            {
                Width = 5,
                HorizontalAlignment = HorizontalAlignment.Right,
                Cursor = Cursors.SizeWE,
                Template = CreateThumbTemplate()
            };
            thumb.DragDelta += (_, e) =>
                column.ColumnWidth = Math.Max(MinColumnWidth, column.ColumnWidth + e.HorizontalChange);
            cell.Children.Add(thumb);

            return cell;
        }

        private static ControlTemplate CreateThumbTemplate()
        {
            var template = new ControlTemplate(typeof(Thumb));
            var rect = new FrameworkElementFactory(typeof(Rectangle));
            rect.SetValue(Shape.FillProperty, Brushes.Transparent);
            rect.SetValue(CursorProperty, Cursors.SizeWE);
            template.VisualTree = rect;
            return template;
        }
    }
}
