using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hekki.UI.Controls
{
    public class NavButton : ListBoxItem
    {
        static NavButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
        }

        public Uri NavLink
        {
            get { return (Uri)GetValue(NavLinkProperty); }
            set { SetValue(NavLinkProperty, value); }
        }

        public static readonly DependencyProperty NavLinkProperty =
            DependencyProperty.Register(nameof(NavLink), typeof(Uri), typeof(NavButton), new PropertyMetadata(null));


        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(NavButton), new PropertyMetadata(null));
    }
}
