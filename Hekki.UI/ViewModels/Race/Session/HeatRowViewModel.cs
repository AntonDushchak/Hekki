using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.DTOs.Race;

namespace Hekki.UI.ViewModels.Race.Session
{
    public partial class HeatRowViewModel : ObservableObject
    {
        public HeatRowViewModel(ParticipantViewModel? participant)
        {
            Participant = participant;
        }

        public ParticipantViewModel? Participant { get; }
        public bool HasParticipant => Participant != null;

        [ObservableProperty] private int? _gridPosition;
        [ObservableProperty] private int? _kartNumber;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(HasResult))] private int? _finishPosition;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(HasResult))] private long? _bestLapMs;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(TotalScore), nameof(HasResult))] private int? _score;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(TotalScore), nameof(HasResult))] private int? _penalty;

        public int TotalScore => (Score ?? 0) - (Penalty ?? 0);

        public bool HasResult => FinishPosition != null || BestLapMs != null || Score != null || Penalty != null;

        public void ApplyEntry(HeatEntryDto entry)
        {
            GridPosition = entry.GridPosition;
            KartNumber = entry.KartNumber;
        }

        public void ApplyResult(HeatResultDto? result)
        {
            FinishPosition = result?.FinishPosition;
            BestLapMs = result?.BestLapMs;
            Score = result?.Score;
            Penalty = result?.Penalty;
        }

        partial void OnKartNumberChanged(int? value) => Participant?.OnResultsChanged();
        partial void OnBestLapMsChanged(long? value) => Participant?.OnResultsChanged();
        partial void OnScoreChanged(int? value) => Participant?.OnResultsChanged();
        partial void OnPenaltyChanged(int? value) => Participant?.OnResultsChanged();
    }
}
