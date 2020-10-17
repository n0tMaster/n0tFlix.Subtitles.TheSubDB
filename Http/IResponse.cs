using System.Collections.Generic;
using System.Net;

namespace n0tFlix.Subtitles.TheSubDB.Http
{
    public interface IResponse
    {
        object Body { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
        HttpStatusCode StatusCode { get; }
    }
}