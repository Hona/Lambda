using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace LambdaUI.Modules
{
    public class ExtraModuleBase : ModuleBase
    {
        // TODO: Check that this is consistent
        public async Task ReplyNewEmbed(string text)
        {
            var parts = text.SplitInParts(2000);
            foreach (var part in parts)
            {
                await ReplyEmbed(EmbedHelper.CreateEmbed(part));
                await Task.Delay(250);
            }
        }

        public async Task ReplyEmbed(Embed embed)
        {
            await ReplyAsync("", embed: embed);
        }

        public async Task ReplyEmbed(EmbedBuilder embed)
        {
            if (embed.Description != null && embed.Description.Length > 2048)
                embed.WithDescription(embed.Description.Take(2000) + "...");
            await ReplyAsync("", embed: embed.Build());
        }
    }
}