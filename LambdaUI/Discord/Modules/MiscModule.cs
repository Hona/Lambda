using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Logging;
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

        public string GetSummaryString(string summary) => string.IsNullOrEmpty(summary) ? "" : $"({summary})";

        [Command("stats")]
        [Summary("Gets various bot-related stats")]
        public async Task Stats()
        {
            var builder = new EmbedBuilder {Title = "**SimplyBot - Stats**"};
            builder.AddField("Uptime", Uptime)
                .AddField("Memory Usage", MemoryUsage)
                .AddField("Latency", ((DiscordSocketClient) Context.Client).Latency)
                .WithFooter($"Discord.Net ({DiscordConfig.Version})");
            await ReplyEmbed(builder);
        }

        [Command("sloc")]
        [Summary("Counts the lines of code in a github repo")]
        public async Task SlocCount(string repo)
        {
            try
            {
                var output = $"cloc-git {repo}".Bash();
                await ReplyNewEmbed("SLOC for repository", output);
            }
            catch (Exception e)
            {
                await ReplyEmbed(Logger.LogException(e));
            }

        }



        [Command("help")]
        [Summary("Displays information about commands")]
        public async Task Help(string moduleParam = "")
        {
            // TODO redo this command
            if (moduleParam == "")
            {
                var title = $"Help commands: ({DiscordConstants.CommandPrefix}help [module])";
                var text = CommandService.Modules.Where(x => !x.Name.Contains("ModuleBase")).Aggregate("",
                    (current, module) =>
                        current +
                        $"  - **{module.Name}** ({module.Summary}), {module.Commands.Count} command/s{Environment.NewLine}  ");
                await ReplyEmbed(EmbedHelper.CreateEmbed(title, text, false));
            }
            else
            {
                var module = CommandService.Modules.First(x => x.Name.ToLower().Contains(moduleParam.ToLower()));
                var title = $"Help: **({module.Name})**";
                var text = module.Commands.Aggregate("",
                    (current, command) =>
                        current +
                        $"**__{DiscordConstants.CommandPrefix + command.Name}__**{Environment.NewLine}**{command.Summary}**. Parameters: {command.Parameters.Aggregate("", (currentString, nextParameter) => currentString + $"{nextParameter.Name} {GetSummaryString(nextParameter.Summary)}, ").TrimEnd(' ', ',')}{Environment.NewLine}");
                await ReplyEmbed(EmbedHelper.CreateEmbed(title, text, false));
            }
        }
    }
}