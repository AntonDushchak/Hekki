using Hekki.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Hekki.UI.Controls
{
    /// <summary>
    /// Логика взаимодействия для RegulationPicker.xaml
    /// </summary>
    public partial class RegulationPicker : UserControl
    {
        public RegulationPicker()
        {
            InitializeComponent();
            DataContext = App.Host.Services.GetRequiredService<RegulationSelectionViewModel>();
        }
    }
}
