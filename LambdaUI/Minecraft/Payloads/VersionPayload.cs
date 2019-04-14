using Newtonsoft.Json;

namespace LambdaUI.Minecraft.Payloads
{
    internal class VersionPayload
    {
        [JsonProperty(PropertyName = "protocol")]
        public int Protocol { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}