using System;
using System.Net.Http;

namespace n0tFlix.Subtitles.TheSubDB.Models
{
    public interface IRequest
    {
        HttpContent Body { get; }
        Uri EndPoint { get; }
        HttpMethod Method { get; }
    }
}