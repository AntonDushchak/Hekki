using Hekki.Application.DTOs.Race;
using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public abstract class HeatGroupColumn : TableColumnViewModel
    {
        protected HeatGroupColumn(double columnWidth) : base(columnWidth)
        {
        }

        public virtual HeatResultField? ResultField => null;

        public abstract string GetValue(HeatRowViewModel row);

        public virtual bool TryParse(string text, out long? value)
        {
            value = null;
            return false;
        }

        public CellViewModel CreateCell(HeatRowViewModel row)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(row)), IsEditableFor(row));
        }

        public void UpdateCell(CellViewModel cell, HeatRowViewModel row)
        {
            var text = GetValue(row);
            if (cell.Value is not TextCellValue current || current.OriginalText != text || current.IsDirty)
                cell.SetValue(CreateTextValue(text));

            cell.IsEditable = IsEditableFor(row);
        }

        private bool IsEditableFor(HeatRowViewModel row) => ResultField != null && row.HasParticipant;
    }

    public abstract class IntegerResultColumn : HeatGroupColumn
    {
        private readonly string _errorKey;

        protected IntegerResultColumn(double columnWidth, string errorKey) : base(columnWidth)
        {
            _errorKey = errorKey;
        }

        public override bool IsNumeric => true;
        public override CellInputKind InputKind => CellInputKind.Integer;

        public override string? ValidateInput(string text) => TryParse(text, out _) ? null : _errorKey;

        public override bool TryParse(string text, out long? value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(text)) return true;
            if (!int.TryParse(text, out var number)) return false;

            value = number;
            return true;
        }
    }

    public class GridPositionColumn : HeatGroupColumn
    {
        public GridPositionColumn() : base(60) { }

        public override string HeaderResourceKey => "m_GridPosition";
        public override bool IsNumeric => true;

        public override string GetValue(HeatRowViewModel row) => row.Entry?.GridPosition?.ToString() ?? string.Empty;
    }

    public class FinishPositionColumn : IntegerResultColumn
    {
        public FinishPositionColumn() : base(60, "err_InvalidPosition") { }

        public override string HeaderResourceKey => "m_FinishPosition";
        public override HeatResultField? ResultField => HeatResultField.FinishPosition;

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
        public override CellInputKind InputKind => CellInputKind.Time;
        public override HeatResultField? ResultField => HeatResultField.BestLap;

        public override string GetValue(HeatRowViewModel row) => LapTimeFormat.Format(row.Result?.BestLapMs);

        public override string? ValidateInput(string text) => TryParse(text, out _) ? null : "err_InvalidTime";

        public override bool TryParse(string text, out long? value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(text)) return true;
            if (!LapTimeFormat.TryParse(text, out var milliseconds)) return false;

            value = milliseconds;
            return true;
        }
    }

    public class ScoreColumn : IntegerResultColumn
    {
        public ScoreColumn() : base(60, "err_InvalidScore") { }

        public override string HeaderResourceKey => "m_Score";
        public override HeatResultField? ResultField => HeatResultField.Score;

        public override string GetValue(HeatRowViewModel row) => row.Result?.Score?.ToString() ?? string.Empty;
    }

    public class PenaltyColumn : IntegerResultColumn
    {
        public PenaltyColumn() : base(70, "err_InvalidPenalty") { }

        public override string HeaderResourceKey => "m_Penalty";
        public override HeatResultField? ResultField => HeatResultField.Penalty;

        public override string GetValue(HeatRowViewModel row) => row.Result?.Penalty?.ToString() ?? string.Empty;
    }
}
