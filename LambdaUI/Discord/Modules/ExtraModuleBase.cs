using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LambdaUI.Utilities;

namespace LambdaUI.Discord.Modules
{
    public class ExtraModuleBase : ModuleBase
    {
        public async Task ReplyNewEmbed(string text, bool escape = true)
        {
            if (escape)
                text = text.EscapeDiscordChars();
            var parts = text.SplitInParts(2000);
            foreach (var part in parts)
            {
                await ReplyEmbed(EmbedHelper.CreateEmbed(part, false));
                await Task.Delay(250);
            }
        }

        public async Task ReplyNewEmbed(string title, string text, bool escape = true)
        {
            if (escape)
                text = text.EscapeDiscordChars();
            var parts = text.SplitInParts(2000);
            foreach (var part in parts)
            {
                await ReplyEmbed(EmbedHelper.CreateEmbed(title, part, false));
                await Task.Delay(250);
            }
        }

        public async Task ReplyEmbed(Embed embed)
        {
            await ReplyAsync("", embed: embed);
        }

        public async Task ReplyEmbed(EmbedBuilder embed)
        {
            if (embed.Description != null && embed.Description.Length >= 2048)
                embed.WithDescription(new string(embed.Description.Take(2000).ToArray()).EscapeDiscordChars() + "...");
            await ReplyAsync("", embed: embed.Build());
        }
    }
}