using Hekki.UI.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Hekki.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;

        public MainWindow(NavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sidebar.SelectedItem as NavButton;

            if (selectedItem?.NavLink != null)
            {
                var page = _navigationService.CreatePage(selectedItem.NavLink);

                if (page != null)
                {
                    mainFrame.Navigate(page);

                    while (mainFrame.CanGoBack) { mainFrame.RemoveBackEntry(); }
                }

            }
        }
    }
}