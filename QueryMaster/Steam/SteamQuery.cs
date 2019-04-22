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

using QueryMaster.Steam.Interfaces;

namespace QueryMaster.Steam
{
    /// <summary>
    ///     Represents Steam's web api interface.
    /// </summary>
    public class SteamQuery
    {
        private PlayerService _iPlayerService;
        private SteamApps _iSteamApps;

        private SteamDirectory _iSteamDirectory;
        private SteamGroup _iSteamGroup;

        private SteamNews _iSteamNews;

        private SteamUser _iSteamUser;

        private SteamUserStats _iSteamUserStats;

        private SteamWebApiUtil _iSteamWebApiUtil;

        /// <summary>
        ///     Initializes Steam's web api interface.
        /// </summary>
        /// <param name="apiKey">Api key.</param>
        public SteamQuery(string apiKey = "") => ApiKey = apiKey;

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
        public SteamApps SteamApps
        {
            get
            {
                if (_iSteamApps == null) _iSteamApps = new SteamApps();
                return _iSteamApps;
            }
        }

        /// <summary>
        ///     Represents the ISteamNews interface.
        /// </summary>
        public SteamNews SteamNews
        {
            get
            {
                if (_iSteamNews == null) _iSteamNews = new SteamNews();
                return _iSteamNews;
            }
        }

        /// <summary>
        ///     Represents the ISteamUser interface.
        /// </summary>
        public SteamUser SteamUser
        {
            get
            {
                if (_iSteamUser == null) _iSteamUser = new SteamUser();
                return _iSteamUser;
            }
        }

        /// <summary>
        ///     Represents the ISteamUserStats interface.
        /// </summary>
        public SteamUserStats SteamUserStats
        {
            get
            {
                if (_iSteamUserStats == null) _iSteamUserStats = new SteamUserStats();
                return _iSteamUserStats;
            }
        }

        /// <summary>
        ///     Represents the ISteamWebAPIUtil interface.
        /// </summary>
        public SteamWebApiUtil SteamWebApiUtil
        {
            get
            {
                if (_iSteamWebApiUtil == null) _iSteamWebApiUtil = new SteamWebApiUtil();
                return _iSteamWebApiUtil;
            }
        }

        /// <summary>
        ///     Represents the ISteamDirectory interface.
        /// </summary>
        public SteamDirectory SteamDirectory
        {
            get
            {
                if (_iSteamDirectory == null) _iSteamDirectory = new SteamDirectory();
                return _iSteamDirectory;
            }
        }

        /// <summary>
        ///     Represents the IPlayerService interface.
        /// </summary>
        public PlayerService PlayerService
        {
            get
            {
                if (_iPlayerService == null) _iPlayerService = new PlayerService();
                return _iPlayerService;
            }
        }

        /// <summary>
        ///     Represents the ISteamGroup interface(not part of steam's web api).
        /// </summary>
        public SteamGroup SteamGroup
        {
            get
            {
                if (_iSteamGroup == null) _iSteamGroup = new SteamGroup();
                return _iSteamGroup;
            }
        }
    }
}