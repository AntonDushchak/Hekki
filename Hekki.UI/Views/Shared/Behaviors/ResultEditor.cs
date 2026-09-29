using Hekki.Application.DTOs.Race;
using Hekki.UI.Services;
using Hekki.UI.ViewModels.Race.Session;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hekki.UI.Views.Shared.Behaviors
{
    public static class ResultEditor
    {
        private static readonly ValidationRule FormatRule = new DataErrorValidationRule();

        public static readonly DependencyProperty FieldProperty =
            DependencyProperty.RegisterAttached(
                "Field",
                typeof(HeatResultField?),
                typeof(ResultEditor),
                new PropertyMetadata(null, OnFieldChanged));

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.RegisterAttached(
                "SaveCommand",
                typeof(ICommand),
                typeof(ResultEditor));

        private static readonly DependencyProperty OriginalTextProperty =
            DependencyProperty.RegisterAttached(
                "OriginalText",
                typeof(string),
                typeof(ResultEditor));

        public static HeatResultField? GetField(DependencyObject element) => (HeatResultField?)element.GetValue(FieldProperty);
        public static void SetField(DependencyObject element, HeatResultField? value) => element.SetValue(FieldProperty, value);

        public static ICommand? GetSaveCommand(DependencyObject element) => (ICommand?)element.GetValue(SaveCommandProperty);
        public static void SetSaveCommand(DependencyObject element, ICommand? value) => element.SetValue(SaveCommandProperty, value);

        public static bool TryParse(HeatResultField field, string text, out long? value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(text)) return true;

            if (field == HeatResultField.BestLap)
            {
                if (!LapTimeFormat.TryParse(text, out var milliseconds)) return false;
                value = milliseconds;
                return true;
            }

            if (!int.TryParse(text, out var number)) return false;
            value = number;
            return true;
        }

        private static string ErrorKey(HeatResultField field) => field switch
        {
            HeatResultField.BestLap => "err_InvalidTime",
            HeatResultField.Score => "err_InvalidScore",
            HeatResultField.Penalty => "err_InvalidPenalty",
            _ => "err_InvalidPosition"
        };

        private static void OnFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.GotKeyboardFocus -= OnGotFocus;
            textBox.LostKeyboardFocus -= OnLostFocus;
            textBox.TextChanged -= OnTextChanged;
            textBox.PreviewKeyDown -= OnPreviewKeyDown;

            if (e.NewValue == null) return;

            textBox.GotKeyboardFocus += OnGotFocus;
            textBox.LostKeyboardFocus += OnLostFocus;
            textBox.TextChanged += OnTextChanged;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
        }

        private static void OnGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SetValue(OriginalTextProperty, textBox.Text);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (GetField(textBox) is not HeatResultField field) return;

            var expression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (expression == null) return;

            if (TryParse(field, textBox.Text, out _))
                Validation.ClearInvalid(expression);
            else
                Validation.MarkInvalid(expression, new ValidationError(FormatRule, expression, Localizer.Get(ErrorKey(field)), null));
        }

        private static void OnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (GetField(textBox) is not HeatResultField field) return;
            if (textBox.DataContext is not HeatRowViewModel { HasParticipant: true } row) return;

            var original = (string?)textBox.GetValue(OriginalTextProperty);
            if (textBox.Text == original) return;
            if (!TryParse(field, textBox.Text, out var value)) return;

            textBox.SetValue(OriginalTextProperty, textBox.Text);

            var request = new ResultEditRequest(row, field, value);
            var command = GetSaveCommand(textBox);
            if (command?.CanExecute(request) == true)
                command.Execute(request);
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;

            if (e.Key == Key.Enter)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                var expression = textBox.GetBindingExpression(TextBox.TextProperty);
                expression?.UpdateTarget();
                if (expression != null) Validation.ClearInvalid(expression);
                textBox.SelectAll();
                e.Handled = true;
            }
        }
    }
}
