using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity
{
    public class MapWr
    {
        [JsonProperty(PropertyName = "record_info")]
        public RecordInfo3 RecordInfo { get; set; }

        [JsonProperty(PropertyName = "zone_info")]
        public ZoneInfo3 ZoneInfo { get; set; }

        [JsonProperty(PropertyName = "map_info")]
        public MapInfo MapInfo { get; set; }

        [JsonProperty(PropertyName = "player_info")]
        public PlayerInfo3 PlayerInfo { get; set; }
    }
}