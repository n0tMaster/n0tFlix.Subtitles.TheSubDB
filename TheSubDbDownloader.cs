using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using n0tFlix.Subtitles.TheSubDB.Configuration;
using n0tFlix.Subtitles.TheSubDB.Helpers;
using n0tFlix.Subtitles.TheSubDB.Http;
using MediaBrowser.Common;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Subtitles;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace n0tFlix.Subtitles.TheSubDB
{
    public class TheSubDbDownloader : ISubtitleProvider
    {

        private static readonly CultureInfo _usCulture = CultureInfo.ReadOnly(new CultureInfo("en-US"));
        private readonly ILogger<TheSubDbDownloader> _logger;
        private readonly IFileSystem _fileSystem;
        private DateTime _lastRateLimitException;
        private DateTime _lastLogin;
        private int _rateLimitLeft = 40;
        private readonly IHttpClient _httpClient;
        private readonly IApplicationHost _appHost;
        private ILocalizationManager _localizationManager;
      
        private readonly IServerConfigurationManager _config;

        private readonly IJsonSerializer _json;
        private readonly SubDBClient _client;

     
        public TheSubDbDownloader(ILogger<TheSubDbDownloader> logger, IHttpClient httpClient, IServerConfigurationManager config, IJsonSerializer json, IFileSystem fileSystem, ILocalizationManager localizationManager)
        {
            _logger = logger;
            _httpClient = httpClient;

            _client = new SubDBClient(new System.Net.Http.Headers.ProductHeaderValue("Desktop-Client", "1.0"));
            _config = config;
            _json = json;
            _fileSystem = fileSystem;
            _localizationManager = localizationManager;
        }
        public int Order => 3;





        public string Name
        {
            get { return "TheSubDB"; }
        }

        private PluginConfiguration GetOptions()
        {
            return Plugin.Instance.Configuration;
        }

        public IEnumerable<VideoContentType> SupportedMediaTypes
        {
            get
            {
                return new[] { VideoContentType.Episode, VideoContentType.Movie };
            }
        }

        private string NormalizeLanguage(string language)
        {
            if (language != null)
            {
                var culture = _localizationManager.FindLanguageInfo(language);
                if (culture != null)
                {
                    return culture.ThreeLetterISOLanguageName;
                }
            }

            return language;
        }


        public void Dispose()
        {
            
        }

        public async Task<IEnumerable<RemoteSubtitleInfo>> Search(SubtitleSearchRequest request, CancellationToken cancellationToken)
        {
            string hash = Helpers.Utils.GetMovieHash(request.MediaPath);
            Response resp =  _client.SearchSubtitleAsync(hash,true).GetAwaiter().GetResult();
            Helpers.CsvResponseParser csvResponseParser = new CsvResponseParser();
            IReadOnlyList<Language> languages = csvResponseParser.ParseSearchSubtitle(ASCIIEncoding.ASCII.GetString((byte[])resp.Body), true);
            List<RemoteSubtitleInfo> list = new List<RemoteSubtitleInfo>();
            _logger.LogInformation("Searched for languages on the hash " + hash + " and found " + languages.Count.ToString() + " results");
            foreach(Language lang in languages)
            {
                
                _logger.LogInformation(lang.Name.ToString() + " " + lang.Count.ToString());
                    _logger.LogInformation("Fant en sub med riktig språk");
                    RemoteSubtitleInfo remoteSubtitleInfo = new RemoteSubtitleInfo()
                    {
                        Name = lang.Name,
                        Id = lang.Name + "-n0t-" + hash,

                    };
                    list.Add(remoteSubtitleInfo);
               
            }
            return list;
        }

        public async Task<SubtitleResponse> GetSubtitles(string id, CancellationToken cancellationToken)
        {
            string[] info = id.Split("-n0t-");
            

            Response response = await _client.DownloadSubtitleAsync(info[1], info[0]).ConfigureAwait(false);

            // download failed
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Failed to download subtitle from TheSubDB");
                return null;
            }
            byte[] buffer = (byte[])response.Body;

            return new SubtitleResponse
            {
                Format = "srt",
                Language = info[0],

                Stream = new MemoryStream(buffer)
            };
           // throw new NotImplementedException();
        }
    }
}
