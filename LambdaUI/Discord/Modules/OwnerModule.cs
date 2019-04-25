using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Data.Access;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Data.Access.Simply;
using LambdaUI.Logging;
using LambdaUI.Services;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(2)]
    [Summary("Commands that you you'll never need")]
    [RequireOwner]
    public class OwnerModule : ExtraModuleBase
    {
        public TempusDataAccess TempusDataAccess { get; set; }
        public DiscordSocketClient Client { get; set; }
        public Lambda Lambda { get; set; }
        public ConfigDataAccess ConfigDataAccess { get; set; }
        public JustJumpDataAccess JustJumpDataAccess { get; set; }

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

        [Command("execjj")]
        public async Task ExecJJ([Remainder] string text)
        {
            var result = await JustJumpDataAccess.QueryAsync(text);
            await ReplyNewEmbed(string.Join(Environment.NewLine, result));
        }

        [Command("updateStatus")]
        public async Task UpdateStatus()
        {
            Lambda.IntervalFunctions(null);
            await ReplyNewEmbed("Done.");
        }

        [Command("giveall")]
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

        [Command("log")]
        public async Task UpdateStatus(LogSeverity severity, [Remainder] string message)
        {
            await Logger.Log(new LogMessage(severity, "Command", message));
        }

        [Command("clearmessages")]
        public async Task ClearMessages()
        {
            if (Context.Channel is ITextChannel channel)
            {
                var messages = await channel.GetMessagesAsync().FlattenAsync();
                await channel.DeleteMessagesAsync(messages);
            }
        }

        [Command("addconfig")]
        public async Task AddConfigEntry(string key, [Remainder] string value)
        {
            await ConfigDataAccess.CreateNewConfigEntryAsync(key, value);
            await ReplyNewEmbed($"Added Key '{key}' and Value '{value}' to config db");
        }

        [Command("deleteconfig")]
        public async Task DeleteConfigEntry(string key, [Remainder] string value)
        {
            await ConfigDataAccess.DeleteConfigEntryAsync(key, value);
            await ReplyNewEmbed($"Deleted Key '{key}' and Value '{value}' from config db");
        }
        [Command("getconfig")]
        public async Task GetAllConfig(string key, [Remainder] string value)
        {
            await ReplyNewEmbed(string.Join(Environment.NewLine, await ConfigDataAccess.GetAllConfigAsync()));
        }
        [Command("bash")]
        public async Task GetAllConfig([Remainder] string command)
        {
            await ReplyNewEmbed(command.Bash());
        }
        [Command("discordid")]
        public async Task GetAllConfig(ulong id)
        {
            await ReplyEmbed(await DiscordService.GetDiscordObjectEmbed(Client, id));
        }
    }
}