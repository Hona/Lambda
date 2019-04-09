using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity {
    public class BonusWr
    {
        [JsonProperty(PropertyName = "record_info")]
        public RecordInfo4 RecordInfo { get; set; }
        [JsonProperty(PropertyName = "zone_info")]
        public ZoneInfo4 ZoneInfo { get; set; }
        [JsonProperty(PropertyName = "map_info")]
        public MapInfo4 MapInfo { get; set; }
        [JsonProperty(PropertyName = "player_info")]
        public PlayerInfo4 PlayerInfo { get; set; }
    }
}