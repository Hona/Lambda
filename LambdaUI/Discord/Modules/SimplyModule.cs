using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Constants;
using LambdaUI.Data.Access.Simply;

namespace LambdaUI.Discord.Modules
{
    [Group("jj")]
    public class SimplyModule : ExtraModuleBase
    {
        public JustJumpDataAccess JustJumpDataAccess { get; set; }

        [Command("dtimes")]
        public async Task GetDemoMapTimes(string mapName)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimes(SimplyConstants.Demoman, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("stimes")]
        public async Task GetSollyMapTimes(string mapName)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimes(SimplyConstants.Soldier, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("ptimes")]
        public async Task GetPyroMapTimes(string mapName)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimes(SimplyConstants.Pyro, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("ctimes")]
        public async Task GetConcMapTimes(string mapName)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimes(SimplyConstants.Conc, mapName)).OrderBy(x => x.RunTime)));
        }

        [Command("etimes")]
        public async Task GetEngiMapTimes(string mapName)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine,
                (await JustJumpDataAccess.GetMapTimes(SimplyConstants.Engineer, mapName)).OrderBy(x => x.RunTime)));
        }
    }
}