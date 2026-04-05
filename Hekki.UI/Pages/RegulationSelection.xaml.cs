using Hekki.UI.ViewModels;
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
            InitializeComponent();
            _viewModel = regulationSelectionViewModel;
            DataContext = _viewModel;
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await _viewModel.InitializeAsync();
        }
    }
}
