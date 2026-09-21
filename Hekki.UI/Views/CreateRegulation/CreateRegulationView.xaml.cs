using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views.CreateRegulation
{
    public partial class CreateRegulationView : UserControl
    {
        public CreateRegulationView()
        {
            InitializeComponent();
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Content is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}
