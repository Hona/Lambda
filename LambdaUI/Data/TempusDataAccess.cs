using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LambdaUI.Logging;
using LambdaUI.Models.Tempus.Rank;
using LambdaUI.Models.Tempus.Responses;
using Newtonsoft.Json;

namespace LambdaUI.Data
{
    public class TempusDataAccess
    {
        private static readonly Stopwatch _stopwatch = new Stopwatch();
        public TempusDataAccess()
        {
            UpdateMapListAsync();
        }

        public List<string> MapList { get; set; }

        private static HttpWebRequest CreateWebRequest(string path)
        {
            return (HttpWebRequest) WebRequest.Create("https://tempus.xyz/api" + path);
        }

        private static HttpWebRequest BuildWebRequest(string relativePath)
        {
            var httpWebRequest = CreateWebRequest(relativePath);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.Accept = "application/json";
            return httpWebRequest;
        }


        private static async Task<T> GetResponseAsync<T>(string request)
        {
            _stopwatch.Restart();
            object stringValue;
            using (var response = (HttpWebResponse) await BuildWebRequest(request).GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    stringValue = null;
                    if (stream != null)
                    {
                        using (var sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            stringValue = sr.ReadToEnd();
                            sr.Close();
                        }
                        stream.Close();
                    }
                }
                response.Close();
            }
            _stopwatch.Stop();
            Logger.LogInfo("Tempus", "/api" + request + " " + _stopwatch.ElapsedMilliseconds +"ms");
            // If T is a string, don't deserialise
            return typeof(T) == typeof(string)
                ? (T) stringValue
                : JsonConvert.DeserializeObject<T>((string) stringValue);
        }
        public async Task<MapFullOverviewModel> GetFullMapOverViewAsync(string map) => await GetResponseAsync<MapFullOverviewModel>($"/maps/name/{ParseMapName(map)}/fullOverview");

        public async Task<RecentActivityModel> GetRecentActivityAsync() => await GetResponseAsync<RecentActivityModel>("/activity");

        public async Task<List<ServerStatusModel>> GetServerStatusAsync() => await GetResponseAsync<List<ServerStatusModel>>("/servers/statusList");

        public async Task<List<ShortMapInfoModel>> GetMapListAsync() => await GetResponseAsync<List<ShortMapInfoModel>>("/maps/list");
        public async Task<Rank> GetUserRank(string id) => await GetResponseAsync<Rank>($"/players/id/{id}/rank");


        private string ParseMapName(string map)
        {
            map = map.ToLower();
            if (MapList.Contains(map)) return map;

            foreach (var mapName in MapList)
            {
                var mapParts = mapName.Split('_');
                if (mapParts.Any(mapPart => map == mapPart)) return mapName;
            }

            throw new Exception("Map not found");
        }

        public async Task UpdateMapListAsync()
        {
            var maps = await GetMapListAsync();
            MapList = maps.ConvertAll(x => x.Name);
        }
    }
}