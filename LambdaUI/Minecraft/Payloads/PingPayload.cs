using Newtonsoft.Json;

namespace LambdaUI.Minecraft.Payloads
{
    /// <summary>
    ///     C# representation of the following JSON file
    ///     https://gist.github.com/thinkofdeath/6927216
    /// </summary>
    internal class PingPayload
    {
        /// <summary>
        ///     Protocol that the server is using and the given name
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public VersionPayload Version { get; set; }

        [JsonProperty(PropertyName = "players")]
        public PlayersPayload Players { get; set; }

        [JsonProperty(PropertyName = "description")]
        public DescriptionPayload Motd { get; set; }

        /// <summary>
        ///     Server icon, important to note that it's encoded in base 64
        /// </summary>
        [JsonProperty(PropertyName = "favicon")]
        public string Icon { get; set; }
    }
}