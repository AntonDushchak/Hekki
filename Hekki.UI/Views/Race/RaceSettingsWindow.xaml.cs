using Hekki.UI.ViewModels;
using System.Windows;

namespace Hekki.UI.Views.Race
{
    public partial class RaceSettingsWindow : Window
    {
        public RaceSettingsWindow()
        {
            InitializeComponent();

            if (DataContext is RaceSettingsViewModel vm)
            {
                vm.CloseAction = () => 
                {
                    DialogResult = vm.DialogResult;
                    Close();
                };
            }
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);

            if (DataContext is RaceSettingsViewModel vm)
            {
                vm.CloseAction = () =>
                {
                    DialogResult = vm.DialogResult;
                    Close();
                };
            }
        }
    }
}
