using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LambdaUI.Models.Tempus.Responses;

namespace LambdaUI.Data
{
    public class TempusDataAccess
    {
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
           
            // If T is a string, don't deserialise
            return typeof(T) == typeof(string)
                ? (T) stringValue
                : JsonConvert.DeserializeObject<T>((string) stringValue);
        }

        public async Task<MapFullOverviewModel> GetFullMapOverViewAsync(string map)
        {
            var response = await GetResponseAsync<MapFullOverviewModel>($"/maps/name/{ParseMapName(map)}/fullOverview");
            return response;
        }
        public async Task<RecentActivityModel> GetRecentActivityAsync()
        {
            var response = await GetResponseAsync<RecentActivityModel>($"/activity");
            return response;
        }
        public async Task<List<ServerStatusModel>> GetServerStatusAsync()
        {
            var response = await GetResponseAsync<List<ServerStatusModel>>($"/servers/statusList");
            return response;
        }

        public async Task<List<ShortMapInfoModel>> GetMapListAsync()
        {
            var response = await GetResponseAsync<List<ShortMapInfoModel>>("/maps/list");
            return response;
        }


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
            MapList = maps.ConvertAll(x=>x.Name);
        }
    }
}