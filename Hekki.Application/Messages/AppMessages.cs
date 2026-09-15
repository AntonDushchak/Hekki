namespace Hekki.Application.Messages
{
    public record AppSuccessMessage(string Message);
    public record AppWarningMessage(string Message);
    public record AppInfoMessage(string Message);
    public record AppErrorMessage(string Message);
}