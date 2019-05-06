using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Data.Access;
using LambdaUI.Discord.Updaters;
using LambdaUI.Models.Tempus.Activity;
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
            await ReplyEmbedAsync(TempusActivityService.GetMapRecordsEmbed(activity.MapRecords));
        }

        [Command("rrtt")]
        public async Task GetRecentTopTimesAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusActivityService.GetMapTopTimesEmbed(activity.MapTopTimes));
        }

        [Command("rrc")]
        public async Task GetRecentCourseRecordsAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusActivityService.GetCourseRecordsEmbed(activity.CourseRecords));
        }

        [Command("rrb")]
        public async Task GetRecentBonusRecordsAsync()
        {
            var activity = await TempusDataAccess.GetRecentActivityAsync();
            await ReplyEmbedAsync(TempusActivityService.GetBonusRecordsEmbed(activity.BonusRecords));
        }

        [Command("dwr")]
        public async Task GetDemoRecordAsync(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbedAsync(
                $"**Demo WR**" + TempusActivityService.FormatRecordSuffix(result, demoRecord), false);
        }

        [Command("dtime")]
        public async Task GetDemoTimeAsync(int place, string map) => await GetDemoTimeAsync(map, place);
        [Command("dtime")]
        public async Task GetDemoTimeAsync(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
            {
                var text = TempusActivityService.FormatRecordSuffix(result, demoRecord);
                await ReplyNewEmbedAsync(
                    $"**Demo #{place}**" + text,
                    false);
            }
            else
                await ReplyNewEmbedAsync("Time not found");
        }

        [Command("swr")]
        public async Task GetSoldierRecordAsync(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbedAsync(
                $"**Soldier WR**" + TempusActivityService.FormatRecordSuffix(result,demoRecord), false);
        }
        [Command("stime")]
        public async Task GetSoldierTimeAsync(int place, string map) => await GetSoldierTimeAsync(map, place);

        [Command("stime")]
        public async Task GetSoldierTimeAsync(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbedAsync(
                    $"**Soldier #{place}**" + TempusActivityService.FormatRecordSuffix(result, demoRecord),
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

        [Alias("m", "mi", "map")]
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

        [Alias("ml")]
        [Command("maplist")]
        public async Task GetMapListAsync(int tier = 0)
        {
            var maps = await TempusDataAccess.GetMapListAsync();
            await ReplyNewEmbedAsync(string.Join(", ", maps));
        }
        //[Command("swrc")]
        //public async Task GetSoldierCourseRecordAsync(string map, int course)
        //{
        //    var result = await TempusDataAccess.GetFullMapOverViewAsync(map);
        //    var soldierRecord = result.SoldierRuns.OrderBy(x => x.Duration).First();
        //    await ReplyNewEmbedAsync(
        //        $"**Soldier WR**" + TempusActivityService.FormatRecordSuffix(result, soldierRecord), false);
        //}
    }
}