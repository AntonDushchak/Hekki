using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hekki.UI.Messages;

namespace Hekki.UI.Services
{
    public sealed class LocalizedText : ObservableObject
    {
        private readonly string _key;
        private readonly object[] _args;
        private readonly string? _fallback;

        public LocalizedText(string key, object[]? args = null, string? fallback = null)
        {
            _key = key;
            _args = args ?? [];
            _fallback = fallback;

            WeakReferenceMessenger.Default.Register<LocalizedText, LanguageChangedMessage>(
                this, (recipient, _) => recipient.OnPropertyChanged(nameof(Value)));
        }

        public string Value => Localizer.TryGet(_key, _args) ?? _fallback ?? _key;

        public override string ToString() => Value;
    }
}
