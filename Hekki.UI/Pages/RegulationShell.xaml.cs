using Hekki.UI.Controls;
using Hekki.UI.ViewModels;
using System.Windows.Controls;

namespace Hekki.UI.Pages
{
    public partial class RegulationShell : Page
    {
        private readonly ShellNavigationService _navigationService;
        public RegulationShell(ShellNavigationService navigationService, RegulationPickerViewModel regulationPicker)
        {
            InitializeComponent();
            _navigationService = navigationService;
            _navigationService.SetFrame(RegulationMainFrame);
            RegulationPicker.DataContext = regulationPicker;
            _ = regulationPicker.InitializeAsync();
        }
    }
}
