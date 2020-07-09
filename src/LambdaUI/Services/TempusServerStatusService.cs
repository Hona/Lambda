using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Logging;
using LambdaUI.Models.Tempus.Responses;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    internal static class TempusServerStatusService
    {
        internal static Embed GetServerStatusAsync(ServerStatusModel server)
        {
            try
            {
                var embed = GetServerEmbed(server);
                return embed;
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        internal static Embed GetServerStatusOverviewEmbed(IEnumerable<ServerStatusModel> servers)
        {
            try
            {
                servers = servers.Where(x => x.GameInfo != null && x.GameInfo.PlayerCount > 0)
                    .OrderByDescending(x => x.GameInfo.PlayerCount);
                var lines = servers.Aggregate("",
                    (currentString, nextServer) => currentString +
                                                   $"[{nextServer.ServerInfo.Name}](https://tempus.xyz/servers/{nextServer.ServerInfo.Id}) " +
                                                   " | (" + nextServer.GameInfo.PlayerCount + "/" +
                                                   nextServer.GameInfo.MaxPlayers + ")" + Environment.NewLine);
                var builder = new EmbedBuilder
                    {
                        Title = "Server Overview", 
                        Description = lines
                    }
                .WithCurrentTimestamp()
                .WithColor(ColorConstants.InfoColor);
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        private static Embed GetServerEmbed(ServerStatusModel server)
        {
            if (server?.ServerInfo == null || server.GameInfo == null || server.ServerInfo.Hidden ||
                server.GameInfo.PlayerCount == 0)
                return null;
            var builder = new EmbedBuilder {Title = $"**{server.ServerInfo.Name}**"};
            builder.AddField("Map", server.GameInfo.CurrentMap.EscapeDiscordChars());
            if (server.GameInfo.NextMap != null)
                builder.AddField("Next Map", server.GameInfo.NextMap.ToString().EscapeDiscordChars());

            builder.AddField("Connect",
                    $"[{server.ServerInfo.Addr}](steam://connect/{server.ServerInfo.Addr}:{server.ServerInfo.Port})"
                        .EscapeDiscordChars())
                .AddField("Players Online", server.GameInfo.PlayerCount + "/" + server.GameInfo.MaxPlayers)
                .WithColor(ColorConstants.InfoColor);
            if (server.GameInfo.Users.Any())
                builder.AddField("Player List",
                    server.GameInfo.Users.OrderBy(x => x.Name).Aggregate("",
                            (currentString, nextPlayer) => currentString + "**" + nextPlayer.Name + "**" + ", ")
                        .TrimEnd(',', ' '));
            return builder.Build();
        }
    }
}