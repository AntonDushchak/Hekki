using Hekki.Application.DTOs.Pilot;
using Hekki.Application.DTOs.Race;
using System.Text.Json.Serialization;

namespace Hekki.Application.Methods
{
    [JsonDerivedType(typeof(RandomShuffle), "random_shuffle")]
    [JsonDerivedType(typeof(ScoreAscShuffle), "score_asc_shuffle")]
    [JsonDerivedType(typeof(TimeDescShuffle), "time_desc_shuffle")]
    [JsonDerivedType(typeof(NoShuffle), "no_shuffle")]
    public interface IParticipantShuffleMethod
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        List<RaceParticipantDto> Shuffle(List<RaceParticipantDto> participants);
    }

    public interface IParticipantShuffleCatalog
    {
        IReadOnlyList<IParticipantShuffleMethod> GetAll();
        IParticipantShuffleMethod GetById(string id);
    }

    public class ParticipantShuffleCatalog : IParticipantShuffleCatalog
    {
        private readonly IReadOnlyList<IParticipantShuffleMethod> _all;
        private readonly IReadOnlyDictionary<string, IParticipantShuffleMethod> _byId;

        public ParticipantShuffleCatalog(IEnumerable<IParticipantShuffleMethod> methods)
        {
            _all = methods
                .OrderBy(m => m.Title)
                .ToList();

            _byId = _all.ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<IParticipantShuffleMethod> GetAll() => _all;

        public IParticipantShuffleMethod GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Shuffle method id is null/empty.", nameof(id));

            if (_byId.TryGetValue(id, out var method))
                return method;

            throw new KeyNotFoundException($"Unknown shuffle method id: '{id}'.");
        }
    }
}
