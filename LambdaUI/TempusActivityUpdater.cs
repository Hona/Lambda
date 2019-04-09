
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data;
using LambdaUI.Models;
using LambdaUI.Models.Tempus.Activity;
using LambdaUI.Models.Tempus.Responses;
using QueryMaster;
using QueryMaster.GameServer;


namespace LambdaUI
{
    public class TempusActivityUpdater
    {
        private readonly DiscordSocketClient _client;
        private ConfigDataAccess _configDataAccess;
        private TempusDataAccess _tempusDataAccess;
        public TempusActivityUpdater(DiscordSocketClient client, ConfigDataAccess configDataAccess, TempusDataAccess tempusDataAccess)
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

        public async Task UpdateActivity()
        {
            var updateChannel = (await _configDataAccess.GetConfigAsync("tempusActivityChannel")).First().Value;
            if (_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)
            {
                await DeleteAllMessages(channel);
                var activity = await _tempusDataAccess.GetRecentActivityAsync();
                await SendMapRecords(activity.MapRecords, channel);
            }

        }

        public async Task SendMapRecords(List<MapWr> records, ITextChannel channel)
        {
            var builder = new EmbedBuilder { Title = $"**Map Records**" };
            var quickRecords = new MapWr[records.Count];
            records.CopyTo(quickRecords);
            var maxLength = quickRecords.OrderByDescending(x => x.MapInfo.Name.Length).First().MapInfo.Name.Length;
            builder.WithDescription(records.Aggregate("", (currentString, nextItem) => currentString += $"{nextItem.RecordInfo.ClassString()} | {nextItem.MapInfo.Name.PadRight(maxLength)} | {nextItem.PlayerInfo.Name}" + Environment.NewLine));
            await channel.SendMessageAsync(embed: builder.Build());
        }
    }
}
