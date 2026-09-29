using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views.Shared.Behaviors
{
    public enum InputKind
    {
        Text,
        Integer,
        Time
    }

    public static class InputFilter
    {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.RegisterAttached(
                "Kind",
                typeof(InputKind),
                typeof(InputFilter),
                new PropertyMetadata(InputKind.Text, OnKindChanged));

        public static InputKind GetKind(DependencyObject element) => (InputKind)element.GetValue(KindProperty);
        public static void SetKind(DependencyObject element, InputKind value) => element.SetValue(KindProperty, value);

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.PreviewTextInput -= OnPreviewTextInput;
            textBox.PreviewKeyDown -= OnPreviewKeyDown;
            DataObject.RemovePastingHandler(textBox, OnPasting);

            var kind = (InputKind)e.NewValue;
            textBox.MaxLength = kind switch
            {
                InputKind.Integer => 3,
                InputKind.Time => 9,
                _ => 0
            };

            if (kind == InputKind.Text) return;

            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(textBox, OnPasting);
        }

        private static bool IsAllowed(InputKind kind, string text) => kind switch
        {
            InputKind.Integer => text.All(char.IsAsciiDigit),
            InputKind.Time => text.All(c => char.IsAsciiDigit(c) || c is ':' or '.' or ','),
            _ => true
        };

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsAllowed(GetKind((DependencyObject)sender), e.Text))
                e.Handled = true;
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (text == null || !IsAllowed(GetKind((DependencyObject)sender), text))
                e.CancelCommand();
        }
    }
}
