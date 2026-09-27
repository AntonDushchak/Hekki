using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public abstract class HeatGroupColumn : TableColumnViewModel
    {
        protected HeatGroupColumn(double columnWidth) : base(columnWidth)
        {
        }

        public abstract string GetValue(HeatRowViewModel row);

        public virtual CellViewModel CreateCell(HeatRowViewModel row)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(row)));
        }

        public virtual void UpdateCell(CellViewModel cell, HeatRowViewModel row)
        {
            cell.SetValue(CreateTextValue(GetValue(row)));
        }
    }

    public class PositionColumn : HeatGroupColumn
    {
        public PositionColumn() : base(50) { }

        public override string HeaderResourceKey => "m_Pos";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => row.Result?.FinishPosition?.ToString() ?? string.Empty;
    }

    public class KartColumn : HeatGroupColumn
    {
        public KartColumn() : base(60) { }

        public override string HeaderResourceKey => "m_Kart";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => row.Entry?.KartNumber?.ToString() ?? string.Empty;
    }

    public class PilotColumn : HeatGroupColumn
    {
        public PilotColumn() : base(180) { }

        public override string HeaderResourceKey => "m_Pilot";

        public override string GetValue(HeatRowViewModel row) => row.Entry?.PilotName ?? string.Empty;
    }

    public class BestLapColumn : HeatGroupColumn
    {
        public BestLapColumn() : base(90) { }

        public override string HeaderResourceKey => "m_Time";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => LapTimeFormat.Format(row.Result?.BestLapMs);
    }

    public class ScoreColumn : HeatGroupColumn
    {
        public ScoreColumn() : base(60) { }

        public override string HeaderResourceKey => "m_Score";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => row.Result?.Score?.ToString() ?? string.Empty;
    }

    public class PenaltyColumn : HeatGroupColumn
    {
        public PenaltyColumn() : base(70) { }

        public override string HeaderResourceKey => "m_Penalty";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => row.Result?.Penalty?.ToString() ?? string.Empty;
    }
}
