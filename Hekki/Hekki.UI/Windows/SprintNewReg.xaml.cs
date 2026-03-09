using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Логика взаимодействия для SprintNewReg.xaml
    /// </summary>
    public partial class SprintNewReg : Window
    {
        private readonly List<int> karts;

        public SprintNewReg(List<int> karts)
        {
            this.karts = karts;
            InitializeComponent();
        }
    }
}
