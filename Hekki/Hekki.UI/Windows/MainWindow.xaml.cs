using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Hekki.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<int> karts = new();

        public MainWindow()
        {
            InitializeComponent();

            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            var version = fileVersionInfo.FileVersion;
            // versionNumber.Text = String.Format("Версия {0}", version.Remove(version.Length - 2));

            numbersOfKarts.TextChanged += NumbersOfKarts_TextChanged;
        }

        private void Cherkasy_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CherkasyReg win = new(karts);
            win.Closed += (s, args) => this.Close();
            win.Show();
        }

        private void SprintOld_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SprintOldReg win = new(karts);
            win.Closed += (s, args) => this.Close();
            win.Show();
        }

        private void SprintNew_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SprintNewReg win = new(karts);
            win.Closed += (s, args) => this.Close();
            win.Show();
        }

        private void TestNew_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            TestNewReg win = new(karts);
            win.Closed += (s, args) => this.Close();
            win.Show();
        }

        private void EveryOnEvery_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            EveryOnEveryReg win = new(karts);
            win.Closed += (s, args) => this.Close();
            win.Show();
        }

        private void NumbersOfKarts_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            karts.Clear();
            var textRange = new TextRange(numbersOfKarts.Document.ContentStart, numbersOfKarts.Document.ContentEnd);
            var text = textRange.Text;

            foreach (string line in text.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (int.TryParse(line.Trim(), out int kartNumber))
                    karts.Add(kartNumber);
            }
        }
    }
}