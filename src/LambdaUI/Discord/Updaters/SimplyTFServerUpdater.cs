using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Logging;
using LambdaUI.Services;

namespace LambdaUI.Discord.Updaters
{
    public class SimplyTFServerUpdater : UpdaterBase
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigDataAccess _configDataAccess;

        public SimplyTFServerUpdater(DiscordSocketClient client, ConfigDataAccess configDataAccess)
        {
            _client = client;
            _configDataAccess = configDataAccess;
        }

        public async Task UpdateServersAsync()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("justjumpUpdateChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var updateChannel in updateChannels)
                await UpdateChannelAsync(updateChannel.Value);
        }

        private async Task UpdateChannelAsync(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                var embeds = new List<Embed>
                {
                    SourceServerStatusService.JustJumpEmbed,
                    SourceServerStatusService.HightowerEmbed,
                    await SourceServerStatusService.GetMinecraftEmbedAsync(),
                    SourceServerStatusService.JumpAcademyEmbed
                };
                
                await DeleteAllMessagesAsync(channel);
                foreach (var embed in embeds)
                {
                    await channel.SendMessageAsync(embed: embed);
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}