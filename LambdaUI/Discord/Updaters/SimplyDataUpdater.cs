using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data;
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
            var updateChannel = (await _configDataAccess.GetConfigAsync("simplyRankUpdateChannel")).First().Value;
            if (_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)
            {
                await DeleteAllMessages(channel);
                await channel.SendMessageAsync(embed: await _simplyDataService.GetHightowerRankEmbedAsync());
                await channel.SendMessageAsync(embed: await _simplyDataService.GetRecentRecordEmbedAsync());
                await channel.SendMessageAsync(embed: await _simplyDataService.GetTopPlayersEmbedAsync());



            }
        }
    }
}
