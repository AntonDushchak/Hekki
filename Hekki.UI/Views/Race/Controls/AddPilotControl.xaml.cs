using Hekki.UI.ViewModels.Race;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views.Race.Controls
{
    public partial class AddPilotControl : UserControl
    {
        private ParticipantsSectionViewModel? Vm => DataContext as ParticipantsSectionViewModel;
        public AddPilotControl()
        {
            InitializeComponent();
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && Vm != null && Vm.IsPopupOpen && SuggestionsList.HasItems)
            {
                SuggestionsList.SelectedIndex = 0;
                (SuggestionsList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem)?.Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                ClosePopupAndClear();
                e.Handled = true;
            }
        }

        private void SuggestionsList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SuggestionsList.SelectedItem != null)
            {
                SelectSuggestion(SuggestionsList.SelectedItem);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (Vm != null) Vm.IsPopupOpen = false;
                SearchTextBox.Focus();
                e.Handled = true;
            }
        }

        private void SuggestionItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                SelectSuggestion(item.DataContext);
                e.Handled = true;
            }
        }

        private void SelectSuggestion(object suggestion)
        {
            Vm?.SelectSuggestionCommand.Execute(suggestion);
            SearchTextBox.Focus();
        }

        private void ClosePopupAndClear()
        {
            if (Vm == null) return;
            Vm.IsPopupOpen = false;
            Vm.SearchText = string.Empty;
        }
    }
}
