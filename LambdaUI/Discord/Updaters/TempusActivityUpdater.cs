using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Data;
using LambdaUI.Models.Tempus.Activity;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Updaters
{
    public class TempusActivityUpdater
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigDataAccess _configDataAccess;
        private readonly TempusDataAccess _tempusDataAccess;

        public TempusActivityUpdater(DiscordSocketClient client, ConfigDataAccess configDataAccess,
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

        public async Task UpdateActivity()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("tempusActivityChannel");
            foreach (var updateChannel in updateChannels.Select(x => x.Value))
            {
                if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) continue;
                await DeleteAllMessages(channel);
                var activity = await _tempusDataAccess.GetRecentActivityAsync();
                await SendMapRecords(activity.MapRecords, channel);
                await SendMapToptimes(activity.MapTopTimes, channel);
            }
        }

        private async Task SendMapToptimes(List<MapTop> topTimes, ITextChannel channel)
        {
            var builder = new EmbedBuilder {Title = $"**Map Top Times**"};
            var quickRecords = new MapTop[topTimes.Count];
            topTimes.CopyTo(quickRecords);
            var description = FormatTopTimes(topTimes.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        private async Task SendMapRecords(List<MapWr> records, ITextChannel channel)
        {
            var builder = new EmbedBuilder {Title = $"**Map Records**"};
            var quickRecords = new MapWr[records.Count];
            records.CopyTo(quickRecords);
            var description = FormatRecords(records.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        private string FormattedDuration(double duration) => new TimeSpan(0, 0, (int) Math.Truncate(duration),
            (int) (duration - (int) Math.Truncate(duration))).ToString("c");

        private string FormatRecords(IEnumerable<MapWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"**{nextItem.RecordInfo.ClassString()}** | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);

        private string FormatTopTimes(IEnumerable<MapTop> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"**{nextItem.RecordInfo.ClassString()} #{nextItem.RecordInfo.Rank}** | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);
    }
}