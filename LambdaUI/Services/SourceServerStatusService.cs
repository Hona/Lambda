using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Minecraft;
using QueryMaster;
using QueryMaster.GameServer;
using Game = QueryMaster.Game;

namespace LambdaUI.Services
{
    public static class SourceServerStatusService
    {
        public static EmbedBuilder JustJumpEmbed => GetEmbedBuilder(ServerConstants.JustJumpServerIpAddress,
            ServerConstants.JustJumpServerPort,
            Game.TeamFortress2);

        public static EmbedBuilder HightowerEmbed => GetEmbedBuilder(ServerConstants.HightowerServerIpAddress,
            ServerConstants.HightowerServerPort,
            Game.TeamFortress2);
        public static EmbedBuilder JumpAcademyEmbed => GetEmbedBuilder(ServerConstants.JumpAcademyServerIpAddress,
            ServerConstants.JumpAcademyServerPort,
            Game.TeamFortress2);


        public static EmbedBuilder SourceEmbed(string ip, ushort port) => GetEmbedBuilder(ip, port, Game.TeamFortress2);

        public static async Task<EmbedBuilder> GetMinecraftEmbed()
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
                .AddField("IP", ServerConstants.MinecraftServerIpAddress)
                .WithColor(ColorConstants.InfoColor);
            if (ping.OnlinePlayerList != null) builder.AddField("Players", string.Join(", ", ping.OnlinePlayerList));
            return builder;
        }
        internal static EmbedBuilder GetEmbedBuilder(string ip, ushort port)
        {
            var server = ServerQuery.GetServerInstance(EngineType.Source, ip, port, sendTimeout: 1000,
                receiveTimeout: 1000, throwExceptions: true);
            return GetSourceServerReplyEmbed(server);
        }

        internal static EmbedBuilder GetEmbedBuilder(string ip, ushort port, Game game)
        {
            var server = ServerQuery.GetServerInstance(game, ip, port, receiveTimeout: 1000, throwExceptions: true);
            return GetSourceServerReplyEmbed(server);
        }

        private static EmbedBuilder GetSourceServerReplyEmbed(Server server)
        {
            var info = server.GetInfo();
            var builder = new EmbedBuilder { Title = $"**{info.Name}**" };
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
