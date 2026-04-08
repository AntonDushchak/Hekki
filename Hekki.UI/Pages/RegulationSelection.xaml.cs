using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hekki.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegulationSelection.xaml
    /// </summary>
    public partial class RegulationSelection : Page
    {
        private readonly RegulationSelectionViewModel _viewModel;

        public RegulationSelection(RegulationSelectionViewModel regulationSelectionViewModel)
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
                // TODO: показать MessageBox / записать в лог
            }
        }
    }
}
