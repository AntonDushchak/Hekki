using Hekki.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views.Controls
{
    public partial class MethodSelectorControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(MethodSelectorControl));

        public static readonly DependencyProperty SelectedMethodIdProperty =
            DependencyProperty.Register(nameof(SelectedMethodId), typeof(string), typeof(MethodSelectorControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AvailableMethodsProperty =
            DependencyProperty.Register(nameof(AvailableMethods), typeof(ObservableCollection<MethodOptionViewModel>), typeof(MethodSelectorControl));

        public static readonly DependencyProperty ShowSettingsCommandProperty =
            DependencyProperty.Register(nameof(ShowSettingsCommand), typeof(ICommand), typeof(MethodSelectorControl));

        public static readonly DependencyProperty SettingsTypeProperty =
            DependencyProperty.Register(nameof(SettingsType), typeof(string), typeof(MethodSelectorControl));

        public static readonly DependencyProperty HasSettingsProperty =
           DependencyProperty.Register(nameof(HasSettings), typeof(bool), typeof(MethodSelectorControl));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(MethodSelectorControl));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string? SelectedMethodId
        {
            get => (string?)GetValue(SelectedMethodIdProperty);
            set => SetValue(SelectedMethodIdProperty, value);
        }

        public ObservableCollection<MethodOptionViewModel> AvailableMethods
        {
            get => (ObservableCollection<MethodOptionViewModel>)GetValue(AvailableMethodsProperty);
            set => SetValue(AvailableMethodsProperty, value);
        }

        public ICommand ShowSettingsCommand
        {
            get => (ICommand)GetValue(ShowSettingsCommandProperty);
            set => SetValue(ShowSettingsCommandProperty, value);
        }

        public string SettingsType
        {
            get => (string)GetValue(SettingsTypeProperty);
            set => SetValue(SettingsTypeProperty, value);
        }

        public bool HasSettings
        {
            get => (bool)GetValue(HasSettingsProperty);
            set => SetValue(HasSettingsProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public MethodSelectorControl()
        {
            InitializeComponent();
        }
    }
}
