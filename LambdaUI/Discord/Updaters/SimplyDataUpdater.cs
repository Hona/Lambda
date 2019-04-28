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

        public async Task UpdateDataAsync()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("simplyRankUpdateChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var channel in updateChannels)
                UpdateChannelAsync(channel.Value);
        }

        private async void UpdateChannelAsync(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                var tasks = new List<Task<Embed>>
                {
                    _simplyDataService.GetHightowerRankEmbedAsync(),
                    _simplyDataService.GetRecentRecordEmbedAsync(),
                    _simplyDataService.GetTopPlayersEmbedAsync()
                };
                var embeds = await Task.WhenAll(tasks);
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