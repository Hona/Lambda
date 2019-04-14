using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Data;
using LambdaUI.Models.Tempus.Responses;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Updaters
{
    public class TempusServerUpdater
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigDataAccess _configDataAccess;
        private readonly TempusDataAccess _tempusDataAccess;

        public TempusServerUpdater(DiscordSocketClient client, ConfigDataAccess configDataAccess,
            TempusDataAccess tempusDataAccess)
        {
            _client = client;
            _configDataAccess = configDataAccess;
            _tempusDataAccess = tempusDataAccess;
        }

        private async Task DeleteAllMessages(ITextChannel channel)
        {
            var messages = await channel.GetMessagesAsync().FlattenAsync();
            await channel.DeleteMessagesAsync(messages);
        }

        public async Task UpdateServers()
        {
            var updateChannel = (await _configDataAccess.GetConfigAsync("tempusUpdateChannel")).First().Value;
            if (_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)
            {
                await DeleteAllMessages(channel);
                var serverInfo = await _tempusDataAccess.GetServerStatusAsync();
                for (var i = 0; i < serverInfo.Count; i++)
                {
                    if (i != 0 && i % 5 == 0)
                        await Task.Delay(3500);
                    var server = serverInfo[i];
                    await UpdateServer(server, channel);
                }
            }
        }

        internal async Task UpdateServer(ServerStatusModel server, ITextChannel channel)
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

        private EmbedBuilder GetServerEmbed(ServerStatusModel server)
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
            return builder;
        }
    }
}