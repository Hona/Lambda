using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data.Access;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Logging;
using LambdaUI.Services;

namespace LambdaUI.Discord.Updaters
{
    public class TempusActivityUpdater : UpdaterBase
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

        public async Task UpdateActivityAsync()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("tempusActivityChannel");
            if (updateChannels == null || updateChannels.Count == 0) return;
            foreach (var updateChannel in updateChannels)
                await UpdateChannelAsync(updateChannel.Value);
        }

        private async Task UpdateChannelAsync(string updateChannel)
        {
            if (!(_client.GetChannel(Convert.ToUInt64(updateChannel)) is ITextChannel channel)) return;
            try
            {
                var activity = await _tempusDataAccess.GetRecentActivityAsync();
                var embeds = new List<Embed>
                {
                    TempusUpdaterService.GetMapRecordsEmbed(activity.MapRecords),
                    TempusUpdaterService.GetMapTopTimesEmbed(activity.MapTopTimes),
                    TempusUpdaterService.GetCourseRecordsEmbed(activity.CourseRecords),
                    TempusUpdaterService.GetBonusRecordsEmbed(activity.BonusRecords)
                };
                await DeleteAllMessagesAsync(channel);
                foreach (var embed in embeds)
                {
                    await channel.SendMessageAsync(embed:embed);
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}