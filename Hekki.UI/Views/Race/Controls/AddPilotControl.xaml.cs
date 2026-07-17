using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class AddPilotControl : UserControl
    {
        private RaceViewModel? Vm => DataContext as RaceViewModel;
        public AddPilotControl()
        {
            InitializeComponent();
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && Vm != null && Vm.IsPopupOpen && SuggestionsList.HasItems)
            {
                SuggestionsList.Focus();
                SuggestionsList.SelectedIndex = 0;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                ClosePopupAndClear();
                e.Handled = true;
            }
        }
        private void SuggestionsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchTextBox.Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (Vm != null) Vm.IsPopupOpen = false;
                SearchTextBox.Focus();
                e.Handled = true;
            }
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!SuggestionsList.IsKeyboardFocusWithin && !SuggestionsList.IsMouseOver)
                {
                    if (Vm != null) Vm.IsPopupOpen = false;
                }
            }), DispatcherPriority.Input);
        }
        private void ClosePopupAndClear()
        {
            if (Vm == null) return;
            Vm.IsPopupOpen = false;
            Vm.SearchText = string.Empty;
        }
    }
}
