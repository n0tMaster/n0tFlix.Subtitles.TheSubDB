using System;
using System.Net.Http;

namespace n0tFlix.Subtitles.TheSubDB.Models
{
    public class Request : IRequest
    {
        public Uri EndPoint { get; set; }
        public HttpMethod Method { get; set; }
        public HttpContent Body { get; set; }
    }
}
