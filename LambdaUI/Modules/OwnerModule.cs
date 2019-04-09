using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Data;

namespace LambdaUI.Modules
{
    [Summary("Commands that you you'll never need")]
    [RequireOwner]
    public class OwnerModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }
        public DiscordSocketClient Client { get; set; }
        public Program Program { get; set; }
        public ConfigDataAccess ConfigDataAccess { get; set; }

        [Command("embed")]
        public async Task Embed([Remainder] string text)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle(text)
                .WithAuthor(text)
                .WithDescription(text)
                .WithFooter(text);
            await ReplyEmbed(builder);
        }
        [Command("maplist")]
        public async Task GetMapList()
        {
            await ReplyNewEmbed(string.Join(", ",(await TempusDataAccess.GetMapList()).ConvertAll(x=>x.Name)));
        }

        [Command("updateStatus")]
        public async Task UpdateStatus()
        {
            Program.IntervalFunctions(null);
            await ReplyNewEmbed("Done.");
        }

        [Command("giveall")]
        [Summary("Executes unescaped SQL queries on the PlayerRanks database")]
        public async Task GiveAll([Remainder] string roleParam)
        {
            var role = Context.Guild.Roles.First(x => x.Name.ToLower().Contains(roleParam.ToLower()));
            var users = (await Context.Guild.GetUsersAsync()).Where(x => !x.IsBot).ToList();
            var count = 0;
            foreach (var user in users)
            {
                count++;
                await user.AddRoleAsync(role);
            }

            await ReplyNewEmbed($"Done adding to {count} non-bot users");
        }

    }
}