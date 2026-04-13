using Hekki.UI.Controls;
using Hekki.UI.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hekki.UI
{
    public partial class MainWindow : Window
    {
        private readonly RootNavigationService _navigationService;

        public MainWindow(RootNavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sidebar.SelectedItem as NavButton;

            if (selectedItem?.NavLink != null)
            {
                _navigationService.SetFrame(mainFrame);
                _navigationService.Navigate(selectedItem.NavLink);
            }
        }
    }
}