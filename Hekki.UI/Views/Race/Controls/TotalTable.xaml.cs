using Hekki.UI.Converters;
using Hekki.UI.ViewModels;
using Hekki.UI.ViewModels.Race.Session;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class TotalTable : UserControl
    {
        private const string DragHandleTag = "DragHandle";

        private readonly List<DataGridColumn> _generatedColumns = [];
        private RaceViewModel? _viewModel;
        private RaceSession? _session;
        private ParticipantViewModel? _dragParticipant;
        private Point _dragStart;
        private InsertionAdorner? _insertionAdorner;

        public TotalTable()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

            _viewModel = DataContext as RaceViewModel;

            if (_viewModel != null)
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            BuildGeneratedColumns();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RaceViewModel.Session))
                BuildGeneratedColumns();
        }

        private void BuildGeneratedColumns()
        {
            var session = _viewModel?.Session;
            if (ReferenceEquals(session, _session)) return;
            _session = session;

            foreach (var column in _generatedColumns)
                PART_Grid.Columns.Remove(column);
            _generatedColumns.Clear();

            if (session == null) return;

            foreach (var heat in session.Heats)
            {
                if (heat.ShowScore)
                    AddColumn(heat.Name, $"HeatRows[{heat.Index}].TotalScore", 50);

                if (heat.ShowTime)
                    AddColumn(heat.Name, $"HeatRows[{heat.Index}].BestLapMs", 90, LapTimeConverter.Instance);
            }

            if (session.Heats.Any(h => h.ShowTime))
                AddColumn(CreateHeader("m_TotalTime"), nameof(ParticipantViewModel.TotalBestLapMs), 90, LapTimeConverter.Instance);

            if (session.Heats.Any(h => h.ShowScore))
                AddColumn(CreateHeader("m_TotalScore"), nameof(ParticipantViewModel.TotalScore), 90);
        }

        private void AddColumn(object header, string path, double width, IValueConverter? converter = null)
        {
            var column = new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding(path) { Mode = BindingMode.OneWay, Converter = converter },
                ElementStyle = (Style)FindResource("CenteredText"),
                Width = width
            };
            PART_Grid.Columns.Add(column);
            _generatedColumns.Add(column);
        }

        private static TextBlock CreateHeader(string resourceKey)
        {
            var header = new TextBlock();
            header.SetResourceReference(TextBlock.TextProperty, resourceKey);
            return header;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragParticipant = null;

            var source = e.OriginalSource as DependencyObject;
            if (FindAncestor<FrameworkElement>(source, el => DragHandleTag.Equals(el.Tag)) == null)
                return;

            _dragParticipant = FindAncestor<DataGridRow>(source)?.Item as ParticipantViewModel;
            _dragStart = e.GetPosition(PART_Grid);
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragParticipant == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(PART_Grid);
            if (Math.Abs(position.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            var participant = _dragParticipant;
            _dragParticipant = null;
            DragDrop.DoDragDrop(PART_Grid, new DataObject(typeof(ParticipantViewModel), participant), DragDropEffects.Move);
            RemoveInsertionAdorner();
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ParticipantViewModel)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            e.Effects = DragDropEffects.Move;
            ShowInsertionAdorner(GetInsertionIndex(e.GetPosition(PART_Grid)));
            e.Handled = true;
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            RemoveInsertionAdorner();
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            RemoveInsertionAdorner();

            if (_viewModel?.Session is not { } session ||
                e.Data.GetData(typeof(ParticipantViewModel)) is not ParticipantViewModel participant)
                return;

            e.Handled = true;

            var oldIndex = session.Participants.IndexOf(participant);
            var insertionIndex = GetInsertionIndex(e.GetPosition(PART_Grid));
            var newIndex = insertionIndex > oldIndex ? insertionIndex - 1 : insertionIndex;

            await _viewModel.ParticipantCommands.MoveAsync(participant, newIndex);
        }

        private int GetInsertionIndex(Point position)
        {
            var count = PART_Grid.Items.Count;
            for (var i = 0; i < count; i++)
            {
                if (PART_Grid.ItemContainerGenerator.ContainerFromIndex(i) is not DataGridRow row)
                    continue;

                var top = row.TranslatePoint(new Point(0, 0), PART_Grid).Y;
                if (position.Y < top + row.ActualHeight / 2)
                    return i;
            }
            return count;
        }

        private double GetInsertionOffset(int insertionIndex)
        {
            var count = PART_Grid.Items.Count;
            if (count == 0) return 0;

            if (insertionIndex < count &&
                PART_Grid.ItemContainerGenerator.ContainerFromIndex(insertionIndex) is DataGridRow row)
                return row.TranslatePoint(new Point(0, 0), PART_Grid).Y;

            if (PART_Grid.ItemContainerGenerator.ContainerFromIndex(count - 1) is DataGridRow last)
                return last.TranslatePoint(new Point(0, last.ActualHeight), PART_Grid).Y;

            return 0;
        }

        private void ShowInsertionAdorner(int insertionIndex)
        {
            if (_insertionAdorner == null)
            {
                var layer = AdornerLayer.GetAdornerLayer(PART_Grid);
                if (layer == null) return;

                var brush = TryFindResource("Brush.ActionDark") as Brush ?? Brushes.DodgerBlue;
                _insertionAdorner = new InsertionAdorner(PART_Grid, brush);
                layer.Add(_insertionAdorner);
            }

            _insertionAdorner.OffsetY = GetInsertionOffset(insertionIndex);
        }

        private void RemoveInsertionAdorner()
        {
            if (_insertionAdorner == null) return;

            AdornerLayer.GetAdornerLayer(PART_Grid)?.Remove(_insertionAdorner);
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
    }
}
