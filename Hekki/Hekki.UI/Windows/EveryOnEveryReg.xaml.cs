using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Логика взаимодействия для EveryOnEveryReg.xaml
    /// </summary>
    public partial class EveryOnEveryReg : Window
    {
        private readonly List<int> karts;

        public EveryOnEveryReg(List<int> karts)
        {
            this.karts = karts;
            InitializeComponent();
        }
    }
}
