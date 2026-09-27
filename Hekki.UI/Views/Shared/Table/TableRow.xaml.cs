using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hekki.UI.Views.Shared.Table
{
    public class CellCommitEventArgs(RoutedEvent routedEvent, object source, TableRowViewModel row, CellViewModel cell)
        : RoutedEventArgs(routedEvent, source)
    {
        public TableRowViewModel Row { get; } = row;
        public CellViewModel Cell { get; } = cell;
    }

    public delegate void CellCommitEventHandler(object sender, CellCommitEventArgs e);

    public partial class TableRow : UserControl
    {
        public static readonly RoutedEvent CellCommitEvent =
            EventManager.RegisterRoutedEvent(
                "CellCommit",
                RoutingStrategy.Bubble,
                typeof(CellCommitEventHandler),
                typeof(TableRow));

        public event CellCommitEventHandler CellCommit
        {
            add => AddHandler(CellCommitEvent, value);
            remove => RemoveHandler(CellCommitEvent, value);
        }

        public TableRow()
        {
            InitializeComponent();
            AddHandler(LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnCellLostFocus));
            AddHandler(PreviewKeyDownEvent, new KeyEventHandler(OnCellKeyDown));
        }

        private void OnCellLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.OriginalSource is not TextBox textBox) return;
            if (DataContext is not TableRowViewModel row) return;
            if (FindCell(textBox) is not CellViewModel cell) return;
            if (cell.Value is not TextCellValue { IsDirty: true, HasErrors: false }) return;

            RaiseEvent(new CellCommitEventArgs(CellCommitEvent, this, row, cell));
        }

        private void OnCellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is not TextBox textBox) return;

            if (e.Key == Key.Enter)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && textBox.DataContext is TextCellValue value)
            {
                value.Revert();
                textBox.SelectAll();
                e.Handled = true;
            }
        }

        private static CellViewModel? FindCell(DependencyObject? element)
        {
            while (element != null)
            {
                if (element is FrameworkElement { DataContext: CellViewModel cell })
                    return cell;

                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }
    }
}
