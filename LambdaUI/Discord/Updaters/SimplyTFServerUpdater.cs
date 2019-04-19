using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data;
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
            var updateChannel = (await _configDataAccess.GetConfigAsync("justjumpUpdateChannel")).First().Value;
            if (_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)
            {
                await DeleteAllMessages(channel);
                await channel.SendMessageAsync(embed: SourceServerStatusService.JustJumpEmbed.Build());
                await channel.SendMessageAsync(embed: SourceServerStatusService.HightowerEmbed.Build());
                await channel.SendMessageAsync(embed: (await SourceServerStatusService.GetMinecraftEmbed()).Build());
                await channel.SendMessageAsync(embed: SourceServerStatusService.JumpAcademyEmbed.Build());

            }
        }
    }
}