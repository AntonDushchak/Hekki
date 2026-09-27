using System.Text.RegularExpressions;

namespace Hekki.UI.Services
{
    public static partial class LapTimeFormat
    {
        [GeneratedRegex(@"^(?:(?<m>\d{2}):)?(?<s>[0-5]\d)[.,](?<ms>\d{3})$")]
        private static partial Regex Pattern();

        public static string Format(long? milliseconds)
        {
            if (!milliseconds.HasValue) return string.Empty;

            var time = TimeSpan.FromMilliseconds(milliseconds.Value);
            var minutes = (int)time.TotalMinutes;

            return minutes > 0
                ? $"{minutes:00}:{time.Seconds:00}.{time.Milliseconds:000}"
                : $"{time.Seconds:00}.{time.Milliseconds:000}";
        }

        public static bool TryParse(string text, out long milliseconds)
        {
            milliseconds = 0;

            var match = Pattern().Match(text.Trim());
            if (!match.Success) return false;

            var minutes = match.Groups["m"].Success ? int.Parse(match.Groups["m"].Value) : 0;
            var seconds = int.Parse(match.Groups["s"].Value);
            var millis = int.Parse(match.Groups["ms"].Value);

            milliseconds = (minutes * 60L + seconds) * 1000 + millis;
            return true;
        }
    }
}
