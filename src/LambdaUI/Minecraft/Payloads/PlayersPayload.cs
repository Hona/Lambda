using System.Collections.Generic;
using Newtonsoft.Json;

namespace LambdaUI.Minecraft.Payloads
{
    internal class PlayersPayload
    {
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        [JsonProperty(PropertyName = "online")]
        public int Online { get; set; }

        [JsonProperty(PropertyName = "sample")]
        public List<Player> Sample { get; set; }
    }
}