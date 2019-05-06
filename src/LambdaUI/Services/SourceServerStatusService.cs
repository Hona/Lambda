using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Logging;
using LambdaUI.Minecraft;
using QueryMaster;
using QueryMaster.GameServer;
using Game = QueryMaster.Game;

namespace LambdaUI.Services
{
    public static class SourceServerStatusService
    {
        public static Embed JustJumpEmbed => GetEmbed(ServerConstants.JustJumpServerIpAddress,
            ServerConstants.JustJumpServerPort,
            Game.TeamFortress2);

        public static Embed HightowerEmbed => GetEmbed(ServerConstants.HightowerServerIpAddress,
            ServerConstants.HightowerServerPort,
            Game.TeamFortress2);

        public static Embed JumpAcademyEmbed => GetEmbed(ServerConstants.JumpAcademyServerIpAddress,
            ServerConstants.JumpAcademyServerPort,
            Game.TeamFortress2);


        public static Embed SourceEmbed(string ip, ushort port) => GetEmbed(ip, port, Game.TeamFortress2);

        public static async Task<Embed> GetMinecraftEmbedAsync()
        {
            try
            {
                var ping = await ServerPing.PingAsync();
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
                    .AddField("Version", ping.Version)
                    .WithColor(ColorConstants.InfoColor);
                if (ping.OnlinePlayerList != null)
                    builder.WithDescription("Players: " + string.Join(", ", ping.OnlinePlayerList));
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        internal static Embed GetEmbed(string ip, ushort port)
        {
            try
            {
                var server = ServerQuery.GetServerInstance(EngineType.Source, ip, port, sendTimeout: 1000,
                    receiveTimeout: 1000, throwExceptions: true);
                return GetSourceServerReplyEmbed(server);
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        private static Embed GetEmbed(string ip, ushort port, Game game)
        {
            try
            {
                try
                {
                    return GetEmbedThrowError(ip, port, game);
                }
                catch
                {
                    Logger.LogWarning("QueryMaster", "Error querying, retrying");
                    // Tries two times, in case the data is malformed due to using UDP
                    return GetEmbedThrowError(ip, port, game);
                }

            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        private static Embed GetEmbedThrowError(string ip, ushort port, Game game)
        {

                var server = ServerQuery.GetServerInstance(game, ip, port, receiveTimeout: 1000, throwExceptions: true);
                return GetSourceServerReplyEmbed(server);

        }

        private static Embed GetSourceServerReplyEmbed(Server server)
        {
            try
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
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }
    }
}