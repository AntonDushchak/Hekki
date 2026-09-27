using Hekki.Application.Exceptions;

namespace Hekki.UI.Services
{
    public static class Localizer
    {
        public static string Get(string key, params object[] args)
        {
            return TryGet(key, args) ?? key;
        }

        public static string? TryGet(string key, params object[] args)
        {
            if (System.Windows.Application.Current?.TryFindResource(key) is not string format)
                return null;

            return args.Length == 0 ? format : string.Format(format, args);
        }

        public static string ForException(Exception exception) => exception is AppException appException
            ? Get(appException.ResourceKey, appException.Args)
            : Get("err_Unexpected");
    }
}
