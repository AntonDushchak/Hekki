using Hekki.UI.ViewModels;
using System.Windows;

namespace Hekki.UI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            DataContext = mainViewModel;

            Loaded += async (s, e) =>
            {
                await _mainViewModel.TopPanelVM.RegulationPicker.InitializeAsync();
            };
        }
    }
}