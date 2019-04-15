using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Data;
using LambdaUI.Models.Tempus.Responses;
using LambdaUI.Services;
using LambdaUI.Utilities;

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
                    await TempusServerStatusService.UpdateServer(serverInfo[i], channel);
                }
            }
        }

    }
}