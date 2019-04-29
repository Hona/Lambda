using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Services;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(0)]
    public class DiscordModule : ExtraModuleBase
    {
        [Command("roleinfo")]
        public async Task GetRoleInfoAsync(string roleParam)
        {
            var embeds = DiscordService.GetRoleEmbeds(roleParam, Context.Guild);
            foreach (var embed in embeds)
            {
                await ReplyEmbedAsync(embed);
            }
        }

        [Command("userinfo")]
        public async Task GetUserInfoAsync(SocketGuildUser userParam)
        {
            await ReplyEmbedAsync(DiscordService.GetGuildUserEmbed(userParam));
        }

        [Command("serverinfo")]
        public async Task GetServerInfoAsync()
        {
            await ReplyEmbedAsync(DiscordService.GetServerEmbed(Context.Guild));
        }


    }
}