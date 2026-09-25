namespace Hekki.Application.Exceptions
{
    public class AppException(string resourceKey, params object[] args)
        : Exception($"{resourceKey}: {string.Join(", ", args)}")
    {
        public string ResourceKey { get; } = resourceKey;
        public object[] Args { get; } = args;
    }
}
