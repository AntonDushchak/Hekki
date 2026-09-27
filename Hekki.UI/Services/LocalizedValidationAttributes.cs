using System.ComponentModel.DataAnnotations;

namespace Hekki.UI.Services
{
    public sealed class LocalizedRequiredAttribute(string resourceKey) : RequiredAttribute
    {
        public override string FormatErrorMessage(string name) => Localizer.Get(resourceKey);
    }

    public sealed class LocalizedMinLengthAttribute(int length, string resourceKey) : MinLengthAttribute(length)
    {
        public override string FormatErrorMessage(string name) => Localizer.Get(resourceKey, Length);
    }

    public sealed class LocalizedMaxLengthAttribute(int length, string resourceKey) : MaxLengthAttribute(length)
    {
        public override string FormatErrorMessage(string name) => Localizer.Get(resourceKey, Length);
    }
}
