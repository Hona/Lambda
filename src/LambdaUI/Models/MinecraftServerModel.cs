using System.Collections.Generic;

namespace LambdaUI.Models
{
    internal class MinecraftServerModel
    {
        public string Version { get; set; }
        public string Protocol { get; set; }
        public string PlayersOnline { get; set; }
        public string PlayersMax { get; set; }
        public List<string> OnlinePlayerList { get; set; }
        public string Motd { get; set; }
    }
}