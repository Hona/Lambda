using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LambdaUI.Constants;
using LambdaUI.Minecraft;
using QueryMaster;
using QueryMaster.GameServer;
using Game = QueryMaster.Game;

namespace LambdaUI.Discord.Modules
{
    [Summary("Gets server info")]
    public class ServerModule : ExtraModuleBase
    {
        public EmbedBuilder JustJumpEmbed => GetEmbedBuilder(ServerConstants.JustJumpServerIpAddress,
            ServerConstants.JustJumpServerPort,
            Game.TeamFortress2);

        public EmbedBuilder HightowerEmbed => GetEmbedBuilder(ServerConstants.HightowerServerIpAddress,
            ServerConstants.HightowerServerPort,
            Game.TeamFortress2);

        public EmbedBuilder GmodEmbed => GetEmbedBuilder(ServerConstants.GmodServerIpAddress,
            ServerConstants.GmodServerPort, Game.GarrysMod);

        public EmbedBuilder SourceEmbed(string ip, ushort port) => GetEmbedBuilder(ip, port, Game.TeamFortress2);

        [Alias("si")]
        [Command("serverinfo")]
        [Summary("Source engine server info")]
        public async Task ServerInfo(string address)
        {
            var ip = address.Split(':')[0];
            if (!ushort.TryParse(address.Split(':')[1], out var port))
            {
                await ReplyNewEmbed("Invalid port number");
                return;
            }

            await ReplyEmbed(GetEmbedBuilder(ip, port));
        }

        [Alias("mc")]
        [Command("minecraft")]
        [Summary("Minecraft server info")]
        public async Task Minecraft()
        {
            var builder = await GetMinecraftEmbed();
            await ReplyEmbed(builder);
        }

        public async Task<EmbedBuilder> GetMinecraftEmbed()
        {
            var ping = await ServerPing.Ping();
            var builder = new EmbedBuilder();

            if (ping.Motd.Contains('§'))
            {
                var split = ping.Motd.Split('§');
                for (var i = 1; i < split.Length; i++) split[i] = string.Join(string.Empty, split[i].Skip(1));

                ping.Motd = string.Join(string.Empty, split);
            }

            builder.WithTitle($"**{ping.Motd}**");
            builder.AddField("Players Online", $"{ping.PlayersOnline}/{ping.PlayersMax}")
                .WithColor(ColorConstants.InfoColor);
            if (ping.OnlinePlayerList != null) builder.AddField("Players", string.Join(", ", ping.OnlinePlayerList));
            return builder;
        }

        [Alias("jj")]
        [Command("justjump")]
        [Summary("JustJust server info")]
        public async Task JustJumpInfo()
        {
            await ReplyEmbed(JustJumpEmbed);
        }

        [Alias("ht")]
        [Command("hightower")]
        [Summary("Hightower server info")]
        public async Task HighTowerInfo()
        {
            await ReplyEmbed(HightowerEmbed);
        }

        [Command("gmod")]
        [Summary("Gmod server info")]
        public async Task GmodInfo()
        {
            await ReplyEmbed(GmodEmbed);
        }

        private EmbedBuilder GetEmbedBuilder(string ip, ushort port)
        {
            var server = ServerQuery.GetServerInstance(EngineType.Source, ip, port, sendTimeout: 1000,
                receiveTimeout: 1000, throwExceptions: true);
            return GetSourceServerReplyEmbed(server);
        }

        private EmbedBuilder GetEmbedBuilder(string ip, ushort port, Game game)
        {
            var server = ServerQuery.GetServerInstance(game, ip, port, receiveTimeout: 1000, throwExceptions: true);
            return GetSourceServerReplyEmbed(server);
        }

        private EmbedBuilder GetSourceServerReplyEmbed(Server server)
        {
            var info = server.GetInfo();
            var builder = new EmbedBuilder {Title = $"**{info.Name}**"};
            builder.AddField("Description", info.Description)
                .AddField("IP", $"[{info.Address}](http://103.1.206.66/tf/redirect/server.php?IP={info.Address})")
                .AddField("Map", info.Map)
                .AddField("Ping", info.Ping)
                .AddField("Players Online", info.Players + "/" + info.MaxPlayers)
                .WithColor(ColorConstants.InfoColor);
            if (server.GetPlayers().Any())
                builder.AddField("Player List",
                    server.GetPlayers().OrderBy(x => x.Name).Aggregate("",
                            (currentString, nextPlayer) => currentString + "**" + nextPlayer.Name + "**" + ", ")
                        .TrimEnd(',', ' '));
            return builder;
        }
    }
}