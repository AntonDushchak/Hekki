using CommunityToolkit.Mvvm.Input;
using Hekki.Application.Abstractions;
using Hekki.Application.Exceptions;
using Hekki.UI.Services;
using System.Diagnostics;

namespace Hekki.UI.ViewModels.Race.Session
{
    public record ParticipantSortOption(string Title, Func<ParticipantViewModel, IComparable?> Key);

    public partial class ParticipantCommands : ViewModelBase
    {
        private readonly IRaceService _raceService;
        private readonly IPilotService _pilotService;
        private readonly IDialogService _dialogService;
        private readonly AppSettings _appSettings;
        private readonly RaceSessionHolder _sessionHolder;

        public ParticipantCommands(
            IRaceService raceService,
            IPilotService pilotService,
            IDialogService dialogService,
            AppSettings appSettings,
            RaceSessionHolder sessionHolder)
        {
            _raceService = raceService;
            _pilotService = pilotService;
            _dialogService = dialogService;
            _appSettings = appSettings;
            _sessionHolder = sessionHolder;
        }

        private RaceSession? Session => _sessionHolder.Current;

        [RelayCommand]
        private Task EditPilotAsync(ParticipantViewModel participant) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var pilot = await _pilotService.GetPilotByIdAsync(participant.PilotId)
                ?? throw new PilotNotFoundException(participant.PilotId);

            var editedPilot = _dialogService.ShowPilotEditor(new PilotEditorViewModel(_appSettings, pilot));
            if (editedPilot == null) return;

            var updated = await _raceService.UpdateParticipantPilotAsync(session.RaceId, participant.Id, editedPilot);
            session.UpdateParticipant(updated);
        });

        [RelayCommand]
        private Task DeleteParticipantAsync(ParticipantViewModel participant) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var confirmed = _dialogService.Confirm(
                Localizer.Get("m_DeleteParticipantTitle"),
                Localizer.Get("m_DeleteParticipantConfirm", participant.FullName),
                Localizer.Get("m_Delete"));
            if (!confirmed) return;

            await _raceService.RemoveParticipantAsync(session.RaceId, participant.Id);
            session.RemoveParticipant(participant.Id);
        });

        [RelayCommand]
        private void ToggleParticipantActive(ParticipantViewModel participant)
        {
            // TODO: exclude/include the participant from the race
        }

        [RelayCommand(CanExecute = nameof(HasProfileLink))]
        private void OpenProfileLink(ParticipantViewModel participant)
        {
            if (TryGetProfileLink(participant, out var link))
                Process.Start(new ProcessStartInfo(link.AbsoluteUri) { UseShellExecute = true });
        }

        private static bool HasProfileLink(ParticipantViewModel? participant) => TryGetProfileLink(participant, out _);

        private static bool TryGetProfileLink(ParticipantViewModel? participant, out Uri link)
        {
            link = null!;
            return Uri.TryCreate(participant?.ProfileUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && (link = uri) != null;
        }

        public IReadOnlyList<ParticipantSortOption> GetSortOptions()
        {
            var options = new List<ParticipantSortOption>
            {
                new(Localizer.Get("m_League"), p => p.League),
                new(Localizer.Get("m_Pilot"), p => p.FullName)
            };

            if (Session is { } session)
            {
                foreach (var heat in session.Heats)
                {
                    var index = heat.Index;
                    if (heat.ShowScore)
                        options.Add(new($"{heat.Name} — {Localizer.Get("m_Score")}", p => p.HeatRows[index]?.TotalScore));
                    if (heat.ShowTime)
                        options.Add(new($"{heat.Name} — {Localizer.Get("m_Time")}", p => p.HeatRows[index]?.BestLapMs));
                }

                if (session.Heats.Any(h => h.ShowTime))
                    options.Add(new(Localizer.Get("m_TotalTime"), p => p.TotalBestLapMs));
                if (session.Heats.Any(h => h.ShowScore))
                    options.Add(new(Localizer.Get("m_TotalScore"), p => p.TotalScore));
            }

            return options;
        }

        [RelayCommand]
        private Task SortAsync(ParticipantSortOption option) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var orderedIds = session.Participants
                .OrderBy(option.Key, Comparer<IComparable?>.Create(CompareSortKeys))
                .Select(p => p.Id)
                .ToList();

            await SaveOrderAsync(session, orderedIds);
        });

        [RelayCommand]
        private Task ReverseAsync() => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var orderedIds = session.Participants.Select(p => p.Id).Reverse().ToList();
            await SaveOrderAsync(session, orderedIds);
        });

        public Task MoveAsync(ParticipantViewModel participant, int newIndex) => ExecuteSafeAsync(async () =>
        {
            if (Session is not { } session) return;

            var oldIndex = session.Participants.IndexOf(participant);
            if (oldIndex < 0 || oldIndex == newIndex || newIndex < 0 || newIndex >= session.Participants.Count) return;

            var orderedIds = session.Participants.Select(p => p.Id).ToList();
            orderedIds.RemoveAt(oldIndex);
            orderedIds.Insert(newIndex, participant.Id);

            await SaveOrderAsync(session, orderedIds);
        });

        private async Task SaveOrderAsync(RaceSession session, IReadOnlyList<Guid> orderedIds)
        {
            await _raceService.ReorderParticipantsAsync(session.RaceId, orderedIds);
            session.ApplyOrder(orderedIds);
        }

        private static int CompareSortKeys(IComparable? left, IComparable? right)
        {
            var leftEmpty = left is null || left is string { Length: 0 };
            var rightEmpty = right is null || right is string { Length: 0 };

            if (leftEmpty || rightEmpty)
                return leftEmpty.CompareTo(rightEmpty);

            if (left is string leftText && right is string rightText)
                return string.Compare(leftText, rightText, StringComparison.CurrentCultureIgnoreCase);

            return left!.CompareTo(right);
        }
    }
}
