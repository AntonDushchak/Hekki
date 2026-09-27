using Hekki.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hekki.UI.Views.Shared.Behaviors
{
    public static class InputFilter
    {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.RegisterAttached(
                "Kind",
                typeof(CellInputKind),
                typeof(InputFilter),
                new PropertyMetadata(CellInputKind.Text, OnKindChanged));

        public static CellInputKind GetKind(DependencyObject element) => (CellInputKind)element.GetValue(KindProperty);
        public static void SetKind(DependencyObject element, CellInputKind value) => element.SetValue(KindProperty, value);

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.PreviewTextInput -= OnPreviewTextInput;
            textBox.PreviewKeyDown -= OnPreviewKeyDown;
            DataObject.RemovePastingHandler(textBox, OnPasting);

            var kind = (CellInputKind)e.NewValue;
            textBox.MaxLength = kind switch
            {
                CellInputKind.Integer => 3,
                CellInputKind.Time => 9,
                _ => 0
            };

            if (kind == CellInputKind.Text) return;

            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(textBox, OnPasting);
        }

        private static bool IsAllowed(CellInputKind kind, string text) => kind switch
        {
            CellInputKind.Integer => text.All(char.IsAsciiDigit),
            CellInputKind.Time => text.All(c => char.IsAsciiDigit(c) || c is ':' or '.' or ','),
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
