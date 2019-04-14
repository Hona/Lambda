using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity
{
    public class MapInfo2
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}