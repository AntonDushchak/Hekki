using CommunityToolkit.Mvvm.ComponentModel;
using Hekki.Application.DTOs.Race;

namespace Hekki.UI.ViewModels.Race.Session
{
    public partial class ParticipantViewModel : ObservableObject
    {
        private HeatRowViewModel?[] _heatRows = [];

        public Guid Id { get; }

        [ObservableProperty] private int _pilotId;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(FullName))] private string _firstName = string.Empty;
        [ObservableProperty, NotifyPropertyChangedFor(nameof(FullName))] private string _lastName = string.Empty;
        [ObservableProperty] private string? _team;
        [ObservableProperty] private string? _league;
        [ObservableProperty] private string? _photoPath;
        [ObservableProperty] private string? _profileUrl;
        [ObservableProperty] private bool _isActive = true;
        [ObservableProperty] private int _order;

        public ParticipantViewModel(RaceParticipantDto dto)
        {
            Id = dto.ParticipantId;
            Apply(dto);
        }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public IReadOnlyList<HeatRowViewModel?> HeatRows => _heatRows;

        public int TotalScore => _heatRows.Sum(row => row?.TotalScore ?? 0);

        public long? TotalBestLapMs
        {
            get
            {
                var laps = _heatRows.Where(row => row?.BestLapMs != null).Select(row => row!.BestLapMs!.Value).ToList();
                return laps.Count == 0 ? null : laps.Sum();
            }
        }

        public string KartNumbers => string.Join(", ", _heatRows.Where(row => row?.KartNumber != null).Select(row => row!.KartNumber));

        public void Apply(RaceParticipantDto dto)
        {
            PilotId = dto.PilotId;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Team = dto.Team;
            League = dto.League;
            PhotoPath = dto.PhotoPath;
            ProfileUrl = dto.ProfileUrl;
            IsActive = dto.IsActive;
        }

        internal void InitHeatRows(int heatCount)
        {
            _heatRows = new HeatRowViewModel?[heatCount];
            OnHeatRowsChanged();
        }

        internal void SetHeatRow(int heatIndex, HeatRowViewModel? row)
        {
            if (ReferenceEquals(_heatRows[heatIndex], row)) return;

            _heatRows[heatIndex] = row;
            OnHeatRowsChanged();
        }

        internal void OnResultsChanged()
        {
            OnPropertyChanged(nameof(TotalScore));
            OnPropertyChanged(nameof(TotalBestLapMs));
            OnPropertyChanged(nameof(KartNumbers));
        }

        private void OnHeatRowsChanged()
        {
            OnPropertyChanged(nameof(HeatRows));
            OnResultsChanged();
        }
    }
}
