namespace Hekki.UI.ViewModels
{
    public enum ColumnType { HeatTime, HeatScore, TotalTime, TotalScore }

    public class ColumnViewModel
    {
        public ColumnType Type { get; init; }
        public string HeaderResourceKey { get; init; } = string.Empty;
        public HeatViewModel? Heat { get; init; }
    }
}
