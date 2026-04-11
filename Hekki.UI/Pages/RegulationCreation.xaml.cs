using Hekki.UI.ViewModels;
using System.Windows.Controls;

namespace Hekki.UI.Pages
{
    public partial class RegulationCreation : Page
    {
        private readonly RegulationCreationViewModel _viewModel;
        public RegulationCreation(RegulationCreationViewModel regulationCreationViewModel)
        {
            _viewModel = regulationCreationViewModel;
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
