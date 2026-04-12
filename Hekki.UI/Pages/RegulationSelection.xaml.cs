using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hekki.UI.Pages
{
    public partial class RegulationSelection : Page
    {
        private readonly RegulationPickerViewModel _viewModel;

        public RegulationSelection(RegulationPickerViewModel regulationSelectionViewModel)
        {
            _viewModel = regulationSelectionViewModel;
            InitializeComponent();
            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.InitializeAsync();
                _viewModel.NavigateToSelected();
            }
            catch (Exception ex)
            {
                // TODO: показать MessageBox
            }
        }
    }
}
