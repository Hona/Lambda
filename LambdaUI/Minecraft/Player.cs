using Newtonsoft.Json;

namespace LambdaUI.Minecraft
{
    internal class Player
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }
    }
}