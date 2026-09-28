using System.Windows;

namespace Hekki.UI.Views
{
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow(string title, string message, string confirmText)
        {
            InitializeComponent();
            Title = title;
            MessageTextBlock.Text = message;
            ConfirmButton.Content = confirmText;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
