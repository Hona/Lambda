using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity
{
    public class CourseWr
    {
        [JsonProperty(PropertyName = "record_info")]
        public RecordInfo2 RecordInfo { get; set; }

        [JsonProperty(PropertyName = "zone_info")]
        public ZoneInfo2 ZoneInfo { get; set; }

        [JsonProperty(PropertyName = "map_info")]
        public MapInfo MapInfo { get; set; }

        [JsonProperty(PropertyName = "player_info")]
        public PlayerInfo2 PlayerInfo { get; set; }
    }
}