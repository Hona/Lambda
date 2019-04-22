using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data.Access;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Logging;
using LambdaUI.Services;

namespace LambdaUI.Discord.Updaters
{
    public class TempusServerUpdater : UpdaterBase
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

        public async Task UpdateOverviewsAsync()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("tempusOverviewChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var updateChannel in updateChannels)
                await UpdateChannelOverviewAsync(updateChannel.Value);
        }

        private async Task UpdateChannelOverviewAsync(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                await DeleteAllMessages(channel);
                await TempusServerStatusService.SendServersStatusOverviewAsync(
                    await _tempusDataAccess.GetServerStatusAsync(), channel);
                await TempusApiService.SendStalkTopEmbedAsync(_tempusDataAccess, channel);
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }

        public async Task UpdateServers()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("tempusUpdateChannel");
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
                var serverInfo = await _tempusDataAccess.GetServerStatusAsync();
                for (var i = 0; i < serverInfo.Count; i++)
                {
                    // Prevent rate limiting
                    if (i != 0 && i % 5 == 0)
                        await Task.Delay(4200);
                    await TempusServerStatusService.SendServerStatusAsync(serverInfo[i], channel);
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}