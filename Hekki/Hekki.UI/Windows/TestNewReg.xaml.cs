using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Логика взаимодействия для TestNewReg.xaml
    /// </summary>
    public partial class TestNewReg : Window
    {
        private readonly List<int> karts;

        public TestNewReg(List<int> karts)
        {
            this.karts = karts;
            InitializeComponent();
        }
    }
}
