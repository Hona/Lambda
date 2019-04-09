#region License

/*
Copyright (c) 2015 Betson Roy

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#endregion

namespace QueryMaster.Steam
{
    /// <summary>
    ///     Represents Steam's web api interface.
    /// </summary>
    public class SteamQuery
    {
        private IPlayerService iPlayerService;
        private ISteamApps iSteamApps;

        private ISteamDirectory iSteamDirectory;
        private ISteamGroup iSteamGroup;

        private ISteamNews iSteamNews;

        private ISteamUser iSteamUser;

        private ISteamUserStats iSteamUserStats;

        private ISteamWebApiUtil iSteamWebApiUtil;

        /// <summary>
        ///     Initializes Steam's web api interface.
        /// </summary>
        /// <param name="apiKey">Api key.</param>
        public SteamQuery(string apiKey = "")
        {
            ApiKey = apiKey;
        }

        /// <summary>
        ///     Api key.
        /// </summary>
        public string ApiKey
        {
            get => SteamUrl.ApiKey;
            set => SteamUrl.ApiKey = value;
        }

        /// <summary>
        ///     Represents the ISteamApps interface.
        /// </summary>
        public ISteamApps ISteamApps
        {
            get
            {
                if (iSteamApps == null) iSteamApps = new ISteamApps();
                return iSteamApps;
            }
        }

        /// <summary>
        ///     Represents the ISteamNews interface.
        /// </summary>
        public ISteamNews ISteamNews
        {
            get
            {
                if (iSteamNews == null) iSteamNews = new ISteamNews();
                return iSteamNews;
            }
        }

        /// <summary>
        ///     Represents the ISteamUser interface.
        /// </summary>
        public ISteamUser ISteamUser
        {
            get
            {
                if (iSteamUser == null) iSteamUser = new ISteamUser();
                return iSteamUser;
            }
        }

        /// <summary>
        ///     Represents the ISteamUserStats interface.
        /// </summary>
        public ISteamUserStats ISteamUserStats
        {
            get
            {
                if (iSteamUserStats == null) iSteamUserStats = new ISteamUserStats();
                return iSteamUserStats;
            }
        }

        /// <summary>
        ///     Represents the ISteamWebAPIUtil interface.
        /// </summary>
        public ISteamWebApiUtil ISteamWebApiUtil
        {
            get
            {
                if (iSteamWebApiUtil == null) iSteamWebApiUtil = new ISteamWebApiUtil();
                return iSteamWebApiUtil;
            }
        }

        /// <summary>
        ///     Represents the ISteamDirectory interface.
        /// </summary>
        public ISteamDirectory ISteamDirectory
        {
            get
            {
                if (iSteamDirectory == null) iSteamDirectory = new ISteamDirectory();
                return iSteamDirectory;
            }
        }

        /// <summary>
        ///     Represents the IPlayerService interface.
        /// </summary>
        public IPlayerService IPlayerService
        {
            get
            {
                if (iPlayerService == null) iPlayerService = new IPlayerService();
                return iPlayerService;
            }
        }

        /// <summary>
        ///     Represents the ISteamGroup interface(not part of steam's web api).
        /// </summary>
        public ISteamGroup ISteamGroup
        {
            get
            {
                if (iSteamGroup == null) iSteamGroup = new ISteamGroup();
                return iSteamGroup;
            }
        }
    }
}