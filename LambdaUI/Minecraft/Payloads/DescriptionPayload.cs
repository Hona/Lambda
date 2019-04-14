using Newtonsoft.Json;

namespace LambdaUI.Minecraft.Payloads
{
    internal class DescriptionPayload
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}