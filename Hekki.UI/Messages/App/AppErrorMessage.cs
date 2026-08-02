namespace Hekki.UI.Messages.App
{
    public record AppErrorMessage(string Message); 
    public record AppSuccessMessage(string Message);
    public record AppInfoMessage(string Message);
    public record AppWarningMessage(string Message);
}