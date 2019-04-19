using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Data;
using LambdaUI.Services;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    public class TempusModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }

        [Command("rr")]
        public async Task GetRecentRecords()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await TempusUpdaterService.SendMapRecords(activity.MapRecords, Context.Channel);
        }

        [Command("rrtt")]
        public async Task GetRecentTopTimes()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await TempusUpdaterService.SendMapTopTimes(activity.MapTopTimes, Context.Channel);
        }

        [Command("rrc")]
        public async Task GetRecentCourseRecords()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await TempusUpdaterService.SendCourseRecords(activity.CourseRecords, Context.Channel);
        }

        [Command("rrb")]
        public async Task GetRecentBonusRecords()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await TempusUpdaterService.SendBonusRecords(activity.BonusRecords, Context.Channel);
        }

        [Command("dwr")]
        public async Task GetDemoRecord(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbed(
                $"**Demo WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}", false);
        }

        [Command("dtime")]
        public async Task GetDemoTime(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbed(
                    $"**Demo #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}",
                    false);
            else
                await ReplyNewEmbed("Time not found");
        }

        [Command("swr")]
        public async Task GetSoldierRecord(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbed(
                $"**Solly WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}", false);
        }

        [Command("stime")]
        public async Task GetSoldierTime(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbed(
                    $"**Solly #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}",
                    false);
            else
                await ReplyNewEmbed("Time not found");
        }

        [Command("stalktop")]
        public async Task StalkTop()
        {
            await TempusApiService.SendStalkTopEmbedAsync(TempusDataAccess, Context.Channel);
        }

        [Command("servers")]
        public async Task ServerOverview()
        {
            await TempusServerStatusService.SendServersStatusOverviewAsync(
                await TempusDataAccess.GetServerStatusAsync(), Context.Channel);
        }

        [Command("ticktime")]
        public async Task TickTime(int tick1, int tick2 = -1)
        {
            if (tick2 == -1)
            {
                await ReplyNewEmbed(TempusHelper.TicksToFormattedTime(tick1));
            }
            else
            {
                var ticks = tick1 > tick2 ? tick1 - tick2 : tick2 - tick1;
                await ReplyNewEmbed(TempusHelper.TicksToFormattedTime(ticks));
            }
        }
        [Command("maplist")]
        public async Task GetMapList(int tier = 0)
        {
            var maps = await TempusDataAccess.GetMapListAsync();
                await ReplyNewEmbed(string.Join(", ", maps));

        }
    }
}