using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Responses
{
    public class ShortMapInfoModel
    {
        [JsonProperty(PropertyName = "id")] public int Id { get; set; }

        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
    }
}
