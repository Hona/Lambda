using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Constants;
using LambdaUI.Data.Access.Simply;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(4)]
    [Group("jj")]
    public class SimplyModule : ExtraModuleBase
    {
        public JustJumpDataAccess JustJumpDataAccess { get; set; }

        [Command("dtimes")]
        public async Task GetDemoMapTimesAsync(string mapName)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimesAsync(SimplyConstants.Demoman, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("stimes")]
        public async Task GetSollyMapTimesAsync(string mapName)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimesAsync(SimplyConstants.Soldier, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("ptimes")]
        public async Task GetPyroMapTimesAsync(string mapName)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimesAsync(SimplyConstants.Pyro, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("ctimes")]
        public async Task GetConcMapTimesAsync(string mapName)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimesAsync(SimplyConstants.Conc, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("etimes")]
        public async Task GetEngiMapTimesAsync(string mapName)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimesAsync(SimplyConstants.Engineer, mapName)).OrderBy(x => x.RunTime)));
        }
    }
}