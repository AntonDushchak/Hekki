using Hekki.UI.Services;

namespace Hekki.UI.ViewModels
{
    public abstract class TotalTableColumn : TableColumnViewModel
    {
        protected TotalTableColumn(double columnWidth = 80) : base(columnWidth)
        {
        }

        public virtual bool IsSortable => true;

        public abstract string GetValue(ParticipantRaceContext context);
        public abstract CellViewModel CreateCell(ParticipantRaceContext context);

        public virtual IComparable? GetSortKey(ParticipantRaceContext context) => GetValue(context);

        public virtual void UpdateCell(CellViewModel cell, ParticipantRaceContext context)
        {
            cell.SetValue(CreateTextValue(GetValue(context)));
        }
    }

    public class RowNumberColumn : TotalTableColumn
    {
        public RowNumberColumn() : base(50) { }

        public override string HeaderResourceKey => "m_RowNumber";
        public override bool IsNumeric => true;

        public override string GetValue(ParticipantRaceContext context) => string.Empty;

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, CreateTextValue(string.Empty));
        }

        public override void UpdateCell(CellViewModel cell, ParticipantRaceContext context)
        {
        }

        public override bool IsSortable => false;
    }

    public class ParticipantColumn : TotalTableColumn
    {
        public override string HeaderResourceKey => "m_Pilot";

        public ParticipantColumn() : base(180) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            var value = new ParticipantCellValue
            {
                Name = context.Participant.FullName,
                KartNumbers = string.Join(", ",
                    context.GetKartNumbers())
            };

            return new CellViewModel(this, value);
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateCell(CellViewModel cell, ParticipantRaceContext context)
        {
            cell.SetValue(new ParticipantCellValue
            {
                Name = context.Participant.FullName,
                KartNumbers = string.Join(", ", context.GetKartNumbers())
            });
        }

        public override IComparable? GetSortKey(ParticipantRaceContext context) => context.Participant.FullName;
    }

    public class PhotoColumn : TotalTableColumn
    {
        public override string HeaderText => "";
        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context))); //TODO: Implement photo cell value
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateCell(CellViewModel cell, ParticipantRaceContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsSortable => false;
    }

    public class LeagueColumn : TotalTableColumn
    {
        public override string HeaderResourceKey => "m_League";

        public LeagueColumn() : base(60) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            return context.Participant.League ?? string.Empty;
        }
    }

    public class TeamColumn : TotalTableColumn
    {
        public override string HeaderResourceKey => "m_Team";

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            return context.Participant.Team ?? string.Empty;
        }
    }

    public class HeatTimeColumn : TotalTableColumn
    {
        public HeatViewModel Heat { get; }

        public HeatTimeColumn(HeatViewModel heat) : base(90)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;
        public override bool IsNumeric => true;

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var row = context.Results[Heat];
            if (row == null) return string.Empty;
            return LapTimeFormat.Format(row.BestLapMs);
        }

        public override IComparable? GetSortKey(ParticipantRaceContext context) => context.Results[Heat]?.BestLapMs;
    }

    public class HeatScoreColumn : TotalTableColumn
    {
        public HeatViewModel Heat { get; }

        public HeatScoreColumn(HeatViewModel heat) : base(50)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;
        public override bool IsNumeric => true;

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var row = context.Results[Heat];
            if (row == null) return string.Empty;
            return row.TotalScore.ToString();
        }

        public override IComparable? GetSortKey(ParticipantRaceContext context) => context.Results[Heat]?.TotalScore;
    }

    public class TotalTimeColumn : TotalTableColumn
    {
        public override string HeaderResourceKey => "m_TotalTime";
        public override bool IsNumeric => true;

        public TotalTimeColumn() : base(90) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var laps = context.Results.Values
                .Where(r => r?.BestLapMs != null)
                .Select(r => r!.BestLapMs!.Value)
                .ToList();

            return laps.Count == 0 ? string.Empty : LapTimeFormat.Format(laps.Sum());
        }

        public override IComparable? GetSortKey(ParticipantRaceContext context)
        {
            var laps = context.Results.Values.Where(r => r?.BestLapMs != null).Select(r => r!.BestLapMs!.Value).ToList();
            return laps.Count == 0 ? null : laps.Sum();
        }
    }

    public class TotalScoreColumn : TotalTableColumn
    {
        public override string HeaderResourceKey => "m_TotalScore";
        public override bool IsNumeric => true;

        public TotalScoreColumn() : base(90) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            return context.Results.Values
                .Where(r => r != null)
                .Sum(r => r!.TotalScore)
                .ToString();
        }

        public override IComparable? GetSortKey(ParticipantRaceContext context)
        {
            return context.Results.Values.Where(r => r != null).Sum(r => r!.TotalScore);
        }
    }

    public class ParticipantRaceContext
    {
        public RaceParticipantViewModel Participant { get; }

        public IReadOnlyDictionary<HeatViewModel, HeatResultViewModel?> Results { get; }

        public IReadOnlyDictionary<HeatViewModel, HeatEntryViewModel?> Entries { get; }

        public ParticipantRaceContext(RaceParticipantViewModel participant, IEnumerable<HeatViewModel> heats)
        {
            Participant = participant;

            Entries = heats.ToDictionary(
                h => h,
                h => h.Groups
                    .SelectMany(g => g.Rows)
                    .FirstOrDefault(r =>
                        r.Entry?.ParticipantId == participant.Id)
                    ?.Entry);

            Results = heats.ToDictionary(
                h => h,
                h => h.Groups
                    .SelectMany(g => g.Rows)
                    .FirstOrDefault(r =>
                        r.Entry?.ParticipantId == participant.Id)
                    ?.Result);
        }

        public IEnumerable<int> GetKartNumbers()
        {
            return Entries.Values
                .Where(e => e?.KartNumber != null)
                .Select(e => e!.KartNumber!.Value);
        }
    }
}
