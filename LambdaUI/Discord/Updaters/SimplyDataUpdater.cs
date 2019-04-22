using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Logging;
using LambdaUI.Services;

namespace LambdaUI.Discord.Updaters
{
    public class SimplyDataUpdater : UpdaterBase
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigDataAccess _configDataAccess;
        private readonly SimplyDataService _simplyDataService;

        public SimplyDataUpdater(DiscordSocketClient client, ConfigDataAccess configDataAccess,
            SimplyDataService simplyDataService)
        {
            _client = client;
            _configDataAccess = configDataAccess;
            _simplyDataService = simplyDataService;
        }

        public async Task UpdateData()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("simplyRankUpdateChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var channel in updateChannels)
                UpdateChannel(channel.Value);
        }

        private async void UpdateChannel(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                await DeleteAllMessages(channel);
                await channel.SendMessageAsync(embed: await _simplyDataService.GetHightowerRankEmbedAsync());
                await channel.SendMessageAsync(embed: await _simplyDataService.GetRecentRecordEmbedAsync());
                await channel.SendMessageAsync(embed: await _simplyDataService.GetTopPlayersEmbedAsync());
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}