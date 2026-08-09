using Hekki.UI.ViewModels;
using Hekki.UI.ViewModels.Race;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class TotalTable : UserControl
    {
        private TotalTableViewModel? _viewModel;

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

            foreach (var column in _viewModel.Columns)
            {
                // Outer Grid: [text *] [thumb 5px]
                var cell = new Grid();
                cell.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                cell.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });

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
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (converter != null)
                    text.SetBinding(TextBlock.TextProperty, new Binding { Source = column, Converter = converter });
                else
                    text.Text = column.HeaderText ?? column.HeaderResourceKey ?? string.Empty;

                Grid.SetColumn(text, 0);
                cell.Children.Add(text);

                // Resize thumb
                var thumb = new Thumb
                {
                    Width = 5,
                    Cursor = Cursors.SizeWE,
                    Template = CreateThumbTemplate()
                };
                var col = column;
                thumb.DragDelta += (_, e) =>
                    col.ColumnWidth = Math.Max(40, col.ColumnWidth + e.HorizontalChange);

                Grid.SetColumn(thumb, 1);
                cell.Children.Add(thumb);

                PART_HeaderPanel.Children.Add(cell);
            }
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
