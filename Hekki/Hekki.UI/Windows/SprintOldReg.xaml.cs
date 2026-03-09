using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Логика взаимодействия для SprintOldReg.xaml
    /// </summary>
    public partial class SprintOldReg : Window
    {
        private readonly List<int> karts;

        public SprintOldReg(List<int> karts)
        {
            this.karts = karts;
            InitializeComponent();
        }
    }
}
