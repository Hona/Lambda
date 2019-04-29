using System;
using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus
{
    public class RecordModel
    {
        public string FormattedDuration => new TimeSpan(0, 0, (int) Math.Truncate(Duration),
            (int) (Duration - (int) Math.Truncate(Duration))).ToString("c");

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "steamid")]
        public string SteamId { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }
    }
}