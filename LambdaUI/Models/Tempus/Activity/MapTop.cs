using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity {
    public class MapTop
    {
        [JsonProperty(PropertyName = "record_info")]
        public RecordInfo RecordInfo { get; set; }
        [JsonProperty(PropertyName = "zone_info")]
        public ZoneInfo ZoneInfo { get; set; }
        [JsonProperty(PropertyName = "map_info")]
        public MapInfo MapInfo { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
        [JsonProperty(PropertyName = "player_info")]
        public PlayerInfo PlayerInfo { get; set; }
    }
}