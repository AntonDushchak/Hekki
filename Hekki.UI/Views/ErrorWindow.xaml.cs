using System.Windows;
using System.Windows.Media;

namespace Hekki.UI.Views
{
    public enum MessageType
    {
        Error,
        Success,
        Info,
        Warning
    }

    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string message, MessageType type = MessageType.Error)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            ConfigureWindowByType(type);
        }

        private void ConfigureWindowByType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    Title = "Error";
                    IconTextBlock.Text = "\uE783"; // ErrorBadge
                    IconTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)); // Red
                    break;
                case MessageType.Success:
                    Title = "Success";
                    IconTextBlock.Text = "\uE73E"; // Completed
                    IconTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x27, 0xAE, 0x60)); // Green
                    break;
                case MessageType.Info:
                    Title = "Information";
                    IconTextBlock.Text = "\uE946"; // Info
                    IconTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB)); // Blue
                    break;
                case MessageType.Warning:
                    Title = "Warning";
                    IconTextBlock.Text = "\uE7BA"; // Warning
                    IconTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0xF3, 0x9C, 0x12)); // Orange
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
