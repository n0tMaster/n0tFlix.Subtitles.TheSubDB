using System.Collections.Generic;

namespace n0tFlix.Subtitles.TheSubDB.Helpers
{
    public interface IResponseParser
    {
        IReadOnlyList<Language> ParseGetAvailablesLanguages(string response);
        IReadOnlyList<Language> ParseSearchSubtitle(string response, bool getVersions);
    }
}
