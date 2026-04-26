using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views
{
    public partial class RegulationCard : UserControl
    {
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(RegulationCard));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(RegulationCard));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public RegulationCard()
        {
            InitializeComponent();
        }
    }
}
