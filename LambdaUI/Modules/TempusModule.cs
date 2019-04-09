using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Data;
using Newtonsoft.Json;

namespace LambdaUI.Modules
{
    public class TempusModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }

        [Command("activity")]
        public async Task GetActivity()
        {
            var result = await TempusDataAccess.GetRecentActivity();
            await ReplyNewEmbed(JsonConvert.SerializeObject(result));
        }
        [Command("status")]
        public async Task GetStatus()
        {
            var result = await TempusDataAccess.GetServerStatus();
            await ReplyNewEmbed(JsonConvert.SerializeObject(result));
        }
        [Command("dwr")]
        public async Task GetDemoRecord(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverView(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbed(
                $"**Demo WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}");
        }

        [Command("dtime")]
        public async Task GetDemoTime(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverView(map);
            var demoRecord = result.DemomanRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbed(
                    $"**Demo #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}");
            else
                await ReplyNewEmbed("Time not found");
        }

        [Command("swr")]
        public async Task GetSoldierRecord(string map)
        {
            var result = await TempusDataAccess.GetFullMapOverView(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).First();
            await ReplyNewEmbed(
                $"**Solly WR** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}");
        }

        [Command("stime")]
        public async Task GetSoldierTime(string map, int place)
        {
            var result = await TempusDataAccess.GetFullMapOverView(map);
            var demoRecord = result.SoldierRuns.OrderBy(x => x.Duration).Skip(place - 1).First();
            if (demoRecord != null)
                await ReplyNewEmbed(
                    $"**Solly #{place}** - {result.MapInfo.Name} - {demoRecord.Name} - {demoRecord.FormattedDuration}");
            else
                await ReplyNewEmbed("Time not found");
        }
    }
}