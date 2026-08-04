namespace Hekki.UI.ViewModels
{
    public class CellViewModel
    {
        public ColumnViewModel Column { get; }
        public HeatResultViewModel? Result { get; }

        public bool IsEditable { get; init; }

        public CellViewModel(ColumnViewModel column, HeatResultViewModel? result, bool isEditable = false)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            Result = result;
            IsEditable = isEditable;
        }

        public string Value
        {
            get
            {
                if (Result == null) return string.Empty;

                return Column.Type switch
                {
                    ColumnType.HeatTime => FormatTime(Result.TotalTimeMs),
                    ColumnType.HeatScore => Result.TotalScore.ToString(),
                    ColumnType.TotalTime => FormatTime(Result.TotalTimeMs),
                    ColumnType.TotalScore => Result.TotalScore.ToString(),
                    _ => string.Empty,
                };
            }
        }

        private static string FormatTime(long? ms)
        {
            if (!ms.HasValue) return string.Empty;
            var ts = TimeSpan.FromMilliseconds(ms.Value);
            return ts.ToString(ts.TotalHours >= 1 ? "h\\:mm\\:ss\\.fff" : "m\\:ss\\.fff");
        }
    }
}
