using CommunityToolkit.Mvvm.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public interface ICellValue
    {
    }

    public partial class TextCellValue : ObservableObject, ICellValue
    {
        [ObservableProperty]
        private string _text;

        public TextCellValue(string text)
        {
            _text = text;
        }
    }

    public class ParticipantCellValue : ICellValue
    {
        public string Name { get; init; } = "";
        public string KartNumbers { get; init; } = "";
    }

    public abstract partial class ColumnViewModel : ObservableObject
    {
        public virtual string? HeaderResourceKey => null;
        public virtual string? HeaderText => null;

        [ObservableProperty]
        private double _columnWidth;

        protected ColumnViewModel(double columnWidth = 80)
        {
            _columnWidth = columnWidth;
        }

        public abstract string GetValue(ParticipantRaceContext context);
        public abstract CellViewModel CreateCell(ParticipantRaceContext context);
        public virtual void UpdateCell(CellViewModel cell, ParticipantRaceContext context)
        {
            cell.SetValue(new TextCellValue(GetValue(context)));
        }
    }

    public class ParticipantColumn : ColumnViewModel
    {
        public override string HeaderResourceKey => "m_Name";

        public ParticipantColumn() : base(180) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            var value = new ParticipantCellValue
            {
                Name = context.Participant.Name,
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
                Name = context.Participant.Name,
                KartNumbers = string.Join(", ", context.GetKartNumbers())
            });
        }
    }

    public class PhotoColumn : ColumnViewModel
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
    }

    public class LeagueColumn : ColumnViewModel
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

    public class TeamColumn : ColumnViewModel
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

    public class HeatTimeColumn : ColumnViewModel
    {
        public HeatViewModel Heat { get; }

        public HeatTimeColumn(HeatViewModel heat)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)), true);
        }

        private static string FormatTime(long? ms)
        {
            if (!ms.HasValue) return string.Empty;
            var ts = TimeSpan.FromMilliseconds(ms.Value);
            return ts.ToString(ts.TotalHours >= 1 ? "h\\:mm\\:ss\\.fff" : "m\\:ss\\.fff");
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var row = context.Results[Heat];
            if (row == null) return string.Empty;
            return FormatTime(row.BestLapMs);
        }
    }

    public class HeatScoreColumn : ColumnViewModel
    {
        public HeatViewModel Heat { get; }

        public HeatScoreColumn(HeatViewModel heat)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)), true);
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var row = context.Results[Heat];
            if (row == null) return string.Empty;
            return row.TotalScore.ToString();
        }
    }

    public class TotalTimeColumn : ColumnViewModel
    {
        public override string HeaderResourceKey => "m_TotalTime";

        public TotalTimeColumn() : base(90) { }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, new TextCellValue(GetValue(context)));
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            return context.Results.Values
               .Where(r => r != null)
               .Sum(r => r!.TotalTimeMs ?? 0)
               .ToString();
        }
    }

    public class TotalScoreColumn : ColumnViewModel
    {
        public override string HeaderResourceKey => "m_TotalScore";

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
    }

    public class ParticipantRaceContext
    {
        public RaceParticipantViewModel Participant { get; }

        public IReadOnlyDictionary<HeatViewModel, HeatResultViewModel?> Results { get; }

        public IReadOnlyDictionary<HeatViewModel, HeatEntryViewModel?> Entries { get; }

        public ParticipantRaceContext(RaceParticipantViewModel participant, IEnumerable<HeatViewModel> heats)
        {
            Participant = participant;

            Results = heats.ToDictionary(
                h => h,
                h => h.Groups
                    .SelectMany(g => g.Rows)
                    .FirstOrDefault(r =>
                        r.Entry.ParticipantId == participant.Id)
                    ?.Result);

            Entries = heats.ToDictionary(
                h => h,
                h => h.Groups
                    .SelectMany(g => g.Rows)
                    .FirstOrDefault(r =>
                        r.Entry.ParticipantId == participant.Id)
                    ?.Entry);
        }

        public IEnumerable<int> GetKartNumbers()
        {
            return Entries.Values
                .Where(e => e?.KartNumber != null)
                .Select(e => e!.KartNumber!.Value);
        }
    }
}
