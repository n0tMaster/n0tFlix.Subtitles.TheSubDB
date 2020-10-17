using System.Threading.Tasks;
using n0tFlix.Subtitles.TheSubDB.Http;

namespace n0tFlix.Subtitles.TheSubDB
{
    public interface ISubDBClient
    {
        Task<Response> DownloadSubtitleAsync(string hash, params string[] languages);
        Task<Response> GetAvailableLanguagesAsync();
        Task<Response> SearchSubtitleAsync(string hash, bool getVersions);
        Task<Response> UploadSubtitleAsync(string subtitle, string movie);
    }
}