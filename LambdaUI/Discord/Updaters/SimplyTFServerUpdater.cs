using System;
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

        public async Task UpdateServers()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("justjumpUpdateChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var updateChannel in updateChannels)
                await UpdateChannel(updateChannel.Value);
        }

        private async Task UpdateChannel(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                await DeleteAllMessages(channel);
                await channel.SendMessageAsync(embed: SourceServerStatusService.JustJumpEmbed);
                await channel.SendMessageAsync(embed: SourceServerStatusService.HightowerEmbed);
                await channel.SendMessageAsync(embed: await SourceServerStatusService.GetMinecraftEmbed());
                await channel.SendMessageAsync(embed: SourceServerStatusService.JumpAcademyEmbed);
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}