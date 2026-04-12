using System.Windows.Controls;

namespace Hekki.UI
{
    public abstract class NavigationServiceBase
    {
        protected Frame? Frame;

        public void SetFrame(Frame frame) => Frame = frame;

        public void Navigate(Page page)
        {
            Frame?.Navigate(page);
            ClearBack();
        }

        protected void ClearBack()
        {
            while (Frame?.CanGoBack == true)
                Frame.RemoveBackEntry();
        }
    }
}
