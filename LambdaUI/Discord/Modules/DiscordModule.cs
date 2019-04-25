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
        public async Task GetRoleInfo(string roleParam)
        {
            var embeds = DiscordService.GetRoleEmbeds(roleParam, Context.Guild);
            foreach (var embed in embeds)
            {
                await ReplyEmbed(embed);
            }
        }

        [Command("userinfo")]
        public async Task GetUserInfo(SocketGuildUser userParam)
        {
            await ReplyEmbed(DiscordService.GetGuildUserEmbed(userParam));
        }

        [Command("serverinfo")]
        public async Task GetServerInfo()
        {
            await ReplyEmbed(DiscordService.GetServerEmbed(Context.Guild));
        }


    }
}