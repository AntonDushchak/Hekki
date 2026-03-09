using System.Windows;

namespace Hekki.UI
{
    /// <summary>
    /// Логика взаимодействия для CherkasyReg.xaml
    /// </summary>
    public partial class CherkasyReg : Window
    {
        private readonly List<int> karts;

        public CherkasyReg(List<int> karts)
        {
            this.karts = karts;
            InitializeComponent();
        }
    }
}
