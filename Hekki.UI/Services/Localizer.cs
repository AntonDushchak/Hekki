using Hekki.Application.Exceptions;

namespace Hekki.UI.Services
{
    public static class Localizer
    {
        public static string Get(string key, params object[] args)
        {
            var format = System.Windows.Application.Current?.TryFindResource(key) as string ?? key;
            return args.Length == 0 ? format : string.Format(format, args);
        }

        public static string ForException(Exception exception) => exception is AppException appException
            ? Get(appException.ResourceKey, appException.Args)
            : Get("err_Unexpected");
    }
}
