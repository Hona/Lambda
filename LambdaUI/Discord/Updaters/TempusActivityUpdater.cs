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

        public async Task UpdateActivity()
        {
            var updateChannels = await _configDataAccess.GetConfigAsync("tempusActivityChannel");
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
                var activity = await _tempusDataAccess.GetRecentActivityAsync();
                await TempusUpdaterService.SendMapRecords(activity.MapRecords, channel);
                await TempusUpdaterService.SendMapTopTimes(activity.MapTopTimes, channel);
                await TempusUpdaterService.SendCourseRecords(activity.CourseRecords, channel);
                await TempusUpdaterService.SendBonusRecords(activity.BonusRecords, channel);
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(embed: Logger.LogException(e));
            }
        }
    }
}