using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Logging;
using LambdaUI.Services;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(1)]
    [Summary("General commands that don't belong in a group")]
    public class MiscModule : ExtraModuleBase
    {
        public CommandService CommandService { get; set; }
        public string MemoryUsage => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2) * 10}MB";


        public string Uptime => 
            $"{(DateTime.Now - Process.GetCurrentProcess().StartTime).Days}d {(DateTime.Now - Process.GetCurrentProcess().StartTime).Hours}h {(DateTime.Now - Process.GetCurrentProcess().StartTime).Minutes}m {(DateTime.Now - Process.GetCurrentProcess().StartTime).Seconds}s";


        [Command("stats")]
        [Summary("Gets various bot-related stats")]
        public async Task StatsAsync()
        {
            var builder = new EmbedBuilder {Title = "**SimplyBot - Stats**"};
            builder.AddField("Uptime", Uptime)
                .AddField("Memory Usage", MemoryUsage)
                .AddField("Latency", ((DiscordSocketClient) Context.Client).Latency)
                .WithFooter($"Discord.Net ({DiscordConfig.Version})");
            await ReplyEmbedAsync(builder);
        }

        [Command("sloc")]
        [Summary("Counts the lines of code in a github repo")]
        public async Task SlocCountAsync(string repo)
        {
            try
            {
                var output = $"cloc-git {repo}".Bash();
                await ReplyNewEmbedAsync("SLOC for repository", output);
            }
            catch (Exception e)
            {
                await ReplyEmbedAsync(Logger.LogException(e));
            }

        }



        [Command("help")]
        [Summary("Displays information about commands")]
        public async Task HelpAsync(string moduleParam = "")
        {
            await ReplyEmbedAsync(DiscordService.GetHelpEmbed(moduleParam, CommandService, Context));
        }
    }
}