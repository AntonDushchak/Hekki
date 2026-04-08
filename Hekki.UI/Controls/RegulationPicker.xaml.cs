using Hekki.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
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

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = App.Host.Services.GetRequiredService<RegulationSelectionViewModel>();
            }
        }
    }
}
