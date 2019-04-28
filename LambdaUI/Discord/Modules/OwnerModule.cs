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
        public async Task EmbedAsync([Remainder] string text)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle(text.EscapeDiscordChars())
                .WithAuthor(text.EscapeDiscordChars())
                .WithDescription(text.EscapeDiscordChars())
                .WithFooter(text.EscapeDiscordChars());
            await ReplyEmbedAsync(builder.Build());
        }

        [Command("execjj")]
        public async Task ExecJJAsync([Remainder] string text)
        {
            var result = await JustJumpDataAccess.QueryAsync(text);
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine, result));
        }

        [Command("updateStatus")]
        public async Task UpdateStatusAsync()
        {
            Lambda.IntervalFunctionsAsync(null);
            await ReplyNewEmbedAsync("Done.");
        }

        [Command("giveall")]
        public async Task GiveAllAsync([Remainder] string roleParam)
        {
            var role = Context.Guild.Roles.First(x => x.Name.ToLower().Contains(roleParam.ToLower()));
            var users = (await Context.Guild.GetUsersAsync()).Where(x => !x.IsBot).ToList();
            var count = 0;
            foreach (var user in users)
            {
                count++;
                await user.AddRoleAsync(role);
            }

            await ReplyNewEmbedAsync($"Done adding to {count} non-bot users");
        }

        [Command("log")]
        public async Task LogAsync(LogSeverity severity, [Remainder] string message)
        {
            await Logger.Log(new LogMessage(severity, "Command", message));
        }

        [Command("clearmessages")]
        public async Task ClearMessagesAsync()
        {
            if (Context.Channel is ITextChannel channel)
            {
                var messages = await channel.GetMessagesAsync().FlattenAsync();
                await channel.DeleteMessagesAsync(messages);
            }
        }

        [Command("addconfig")]
        public async Task AddConfigEntryAsync(string key, [Remainder] string value)
        {
            await ConfigDataAccess.CreateNewConfigEntryAsync(key, value);
            await ReplyNewEmbedAsync($"Added Key '{key}' and Value '{value}' to config db");
        }

        [Command("deleteconfig")]
        public async Task DeleteConfigEntryAsync(string key, [Remainder] string value)
        {
            await ConfigDataAccess.DeleteConfigEntryAsync(key, value);
            await ReplyNewEmbedAsync($"Deleted Key '{key}' and Value '{value}' from config db");
        }
        [Command("getconfig")]
        public async Task GetAllConfigAsync(string key, [Remainder] string value)
        {
            await ReplyNewEmbedAsync(string.Join(Environment.NewLine, await ConfigDataAccess.GetAllConfigAsync()));
        }
        [Command("bash")]
        public async Task BashAsync([Remainder] string command)
        {
            await ReplyNewEmbedAsync(command.Bash());
        }
        [Command("discordid")]
        public async Task GetAllConfigAsync(ulong id)
        {
            await ReplyEmbedAsync(await DiscordService.GetDiscordObjectEmbedAsync(Client, id));
        }
    }
}