using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity
{
    public class MapInfo
    {
        [JsonProperty(PropertyName = "date_added")]
        public double? DateAdded { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}