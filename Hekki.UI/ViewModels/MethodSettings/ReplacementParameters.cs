using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public partial class ReplacementParameters : MethodParameters
    {
        [ObservableProperty] private int _numberToDown = 2;

        [ObservableProperty] private int _numberToUp = 2;
    }
}
