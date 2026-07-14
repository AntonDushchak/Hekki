using Hekki.Application.DTOs.Pilot;

namespace Hekki.Application.Methods
{
    public class RandomShuffle : IParticipantShuffleMethod
    {
        public string Id => "random_shuffle";
        public string Title => "Random Shuffle";
        public string Description => "Randomly shuffles the pilots.";


        public List<PilotDto> Shuffle(List<PilotDto> participants)
        {
            throw new NotImplementedException();
        }
    }

    public class ScoreAscShuffle : IParticipantShuffleMethod
    {
        public string Id => "score_asc_shuffle";
        public string Title => "Score Ascending Shuffle";
        public string Description => "Shuffles the pilots based on their scores in ascending order.";

        public List<PilotDto> Shuffle(List<PilotDto> participants)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeDescShuffle : IParticipantShuffleMethod
    {
        public string Id => "time_desc_shuffle";
        public string Title => "Time Descending Shuffle";
        public string Description => "Shuffles the pilots based on their times in descending order.";

        public List<PilotDto> Shuffle(List<PilotDto> participants)
        {
            throw new NotImplementedException();
        }
    }

    public class NoShuffle : IParticipantShuffleMethod
    {
        public string Id => "no_shuffle";
        public string Title => "No Shuffle";
        public string Description => "Does not shuffle the pilots.";

        public List<PilotDto> Shuffle(List<PilotDto> participants)
        {
            return participants;
        }
    }
}
