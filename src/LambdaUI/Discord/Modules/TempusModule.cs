using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Data.Access;
using LambdaUI.Services;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(5)]
    public class TempusModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }

        [Command("rr")]
        public async Task GetRecentRecordsAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusUpdaterService.GetMapRecordsEmbed(activity.MapRecords));
        }

        [Command("rrtt")]
        public async Task GetRecentTopTimesAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusUpdaterService.GetMapTopTimesEmbed(activity.MapTopTimes));
        }

        [Command("rrc")]
        public async Task GetRecentCourseRecordsAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusUpdaterService.GetCourseRecordsEmbed(activity.CourseRecords));
        }

        [Command("rrb")]
        public async Task GetRecentBonusRecordsAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusUpdaterService.GetBonusRecordsEmbed(activity.BonusRecords));
        }

        [Command("dwr")]
        public async Task GetDemoRecordAsync(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbedAsync(
                $"**Demo WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}", false);
        }

        [Command("dtime")]
        public async Task GetDemoTimeAsync(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbedAsync(
                    $"**Demo #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}",
                    false);
            else
                await ReplyNewEmbedAsync("Time not found");
        }

        [Command("swr")]
        public async Task GetSoldierRecordAsync(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbedAsync(
                $"**Solly WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}", false);
        }

        [Command("stime")]
        public async Task GetSoldierTimeAsync(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbedAsync(
                    $"**Solly #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}",
                    false);
            else
                await ReplyNewEmbedAsync("Time not found");
        }

        [Command("stalktop")]
        public async Task StalkTopAsync()
        {
            await ReplyEmbedAsync( await TempusApiService.GetStalkTopEmbedAsync(TempusDataAccess));
        }

        [Command("servers")]
        public async Task ServerOverviewAsync()
        {
            await ReplyEmbedAsync(TempusServerStatusService.GetServerStatusOverviewEmbed(await TempusDataAccess.GetServerStatusAsync()));
        }

        [Alias("m")]
        [Command("mapinfo")]
        public async Task MapInfoAsync(string mapName)
        {
            var map = TempusDataAccess.MapList.First(x => x.Name.Contains(mapName));
            await ReplyAsync(embed: TempusApiService.GetMapInfoEmbed(map));
        }

        [Command("ticktime")]
        public async Task TickTimeAsync(int tick1, int tick2 = -1)
        {
            if (tick2 == -1)
            {
                await ReplyNewEmbedAsync(TempusHelper.TicksToFormattedTime(tick1));
            }
            else
            {
                var ticks = tick1 > tick2 ? tick1 - tick2 : tick2 - tick1;
                await ReplyNewEmbedAsync(TempusHelper.TicksToFormattedTime(ticks));
            }
        }

        [Command("maplist")]
        public async Task GetMapListAsync(int tier = 0)
        {
            var maps = await TempusDataAccess.GetMapListAsync();
            await ReplyNewEmbedAsync(string.Join(", ", maps));
        }
    }
}