using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.UI.Services;
using System.Collections;
using System.ComponentModel;

namespace Hekki.UI.ViewModels
{
    public interface ICellValue
    {
    }

    public enum CellInputKind
    {
        Text,
        Integer,
        Time
    }

    public partial class TextCellValue : ObservableObject, ICellValue, INotifyDataErrorInfo
    {
        private readonly Func<string, string?>? _validate;
        private string? _error;

        [ObservableProperty]
        private string _text;

        public CellInputKind InputKind { get; }

        public TextCellValue(string text, CellInputKind inputKind = CellInputKind.Text, Func<string, string?>? validate = null)
        {
            _text = text;
            InputKind = inputKind;
            _validate = validate;
        }

        public bool HasErrors => _error != null;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return propertyName == nameof(Text) && _error != null ? new[] { _error } : Array.Empty<string>();
        }

        partial void OnTextChanged(string value)
        {
            var errorKey = _validate?.Invoke(value);
            var error = errorKey == null ? null : Localizer.Get(errorKey);
            if (error == _error) return;

            _error = error;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
            OnPropertyChanged(nameof(HasErrors));
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
        public virtual bool IsNumeric => false;
        public virtual CellInputKind InputKind => CellInputKind.Text;

        public virtual string? ValidateInput(string text) => null;

        protected TextCellValue CreateTextValue(string text) => new(text, InputKind, ValidateInput);

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
            cell.SetValue(CreateTextValue(GetValue(context)));
        }
    }

    public class ParticipantColumn : ColumnViewModel
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

        public HeatTimeColumn(HeatViewModel heat) : base(90)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;
        public override bool IsNumeric => true;
        public override CellInputKind InputKind => CellInputKind.Time;

        public override string? ValidateInput(string text)
        {
            return string.IsNullOrWhiteSpace(text) || LapTimeFormat.TryParse(text, out _) ? null : "err_InvalidTime";
        }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(context)), true);
        }

        public override string GetValue(ParticipantRaceContext context)
        {
            var row = context.Results[Heat];
            if (row == null) return string.Empty;
            return LapTimeFormat.Format(row.BestLapMs);
        }
    }

    public class HeatScoreColumn : ColumnViewModel
    {
        public HeatViewModel Heat { get; }

        public HeatScoreColumn(HeatViewModel heat) : base(50)
        {
            Heat = heat;
        }

        public override string HeaderText => Heat.Name;
        public override bool IsNumeric => true;
        public override CellInputKind InputKind => CellInputKind.Integer;

        public override string? ValidateInput(string text)
        {
            return string.IsNullOrWhiteSpace(text) || int.TryParse(text, out _) ? null : "err_InvalidScore";
        }

        public override CellViewModel CreateCell(ParticipantRaceContext context)
        {
            return new CellViewModel(this, CreateTextValue(GetValue(context)), true);
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
        public override bool IsNumeric => true;

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
