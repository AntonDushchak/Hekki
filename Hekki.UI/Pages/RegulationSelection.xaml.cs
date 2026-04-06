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

            Loaded += async (_, __) =>
            {
                try
                {
                    await _viewModel.InitializeAsync();
                }
                catch (Exception ex)
                {
                    // TODO: показать MessageBox / записать в лог
                }
            };
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            try
            {
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                // TODO: показать MessageBox / записать в лог
            }
        }
    }
}
