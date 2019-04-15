using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Data;
using LambdaUI.Logging;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    [Summary("Commands that you you'll never need")]
    [RequireOwner]
    public class OwnerModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }
        public DiscordSocketClient Client { get; set; }
        public Lambda Lambda { get; set; }
        public ConfigDataAccess ConfigDataAccess { get; set; }

        [Command("embed")]
        public async Task Embed([Remainder] string text)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle(text.EscapeDiscordChars())
                .WithAuthor(text.EscapeDiscordChars())
                .WithDescription(text.EscapeDiscordChars())
                .WithFooter(text.EscapeDiscordChars());
            await ReplyEmbed(builder.Build());
        }

        [Command("maplist")]
        public async Task GetMapList()
        {
            await ReplyNewEmbed(string.Join(", ", (await TempusDataAccess.GetMapListAsync()).ConvertAll(x => x.Name)));
        }

        [Command("updateStatus")]
        public async Task UpdateStatus()
        {
            Lambda.IntervalFunctions(null);
            await ReplyNewEmbed("Done.");
        }

        [Command("giveall")]
        [Summary("Executes unescaped SQL queries on the PlayerRanks database")]
        public async Task GiveAll([Remainder] string roleParam)
        {
            var role = Enumerable.First<IRole>(Context.Guild.Roles, x => x.Name.ToLower().Contains(roleParam.ToLower()));
            var users = Enumerable.Where<IGuildUser>((await Context.Guild.GetUsersAsync()), x => !x.IsBot).ToList();
            var count = 0;
            foreach (var user in users)
            {
                count++;
                await user.AddRoleAsync(role);
            }

            await ReplyNewEmbed($"Done adding to {count} non-bot users");
        }
        [Command("log")]
        public async Task UpdateStatus(LogSeverity severity, [Remainder] string message)
        {
            await Logger.Log(new LogMessage(severity, "Command", message));
        }
    }
}