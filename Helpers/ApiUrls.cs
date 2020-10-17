﻿using System;

namespace n0tFlix.Subtitles.TheSubDB.Helpers
{
    public static class ApiUrls
    {
        /// <summary>
        /// Uri used for development and testing.
        /// </summary>
        public static readonly Uri SubDBApiSandBoxUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);

        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);

    }
}
