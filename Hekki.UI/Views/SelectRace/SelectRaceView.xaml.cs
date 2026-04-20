using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hekki.UI.Views
{
    public partial class SelectRaceView : UserControl
    {
        public SelectRaceView()
        {
            InitializeComponent();
        }

        private async void SelectRaceView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SelectionViewModel vm)
            {
                await vm.InitializeAsync();
            }
        }
    }
}
