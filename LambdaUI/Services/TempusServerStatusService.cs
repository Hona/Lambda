using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Models.Tempus.Responses;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    internal static class TempusServerStatusService
    {
        internal static async Task UpdateServer(ServerStatusModel server, ITextChannel channel)
        {
            try
            {
                var builder = GetServerEmbed(server);
                if (builder == null) return;


                await channel.SendMessageAsync(embed: builder.Build());
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.Message);
            }
        }

        private static EmbedBuilder GetServerEmbed(ServerStatusModel server)
        {
            if (server?.ServerInfo == null || server.GameInfo == null || server.ServerInfo.Hidden ||
                server.GameInfo.PlayerCount == 0)
                return null;
            var builder = new EmbedBuilder { Title = $"**{server.ServerInfo.Name}**" };
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
            return builder;
        }
    }
}
