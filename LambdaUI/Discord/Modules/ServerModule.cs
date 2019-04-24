using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Services;

namespace LambdaUI.Discord.Modules
{
    [ModuleId(3)]
    [Summary("Gets server info")]
    public class ServerModule : ExtraModuleBase
    {
        [Alias("si")]
        [Command("serverinfo")]
        [Summary("Source engine server info")]
        public async Task ServerInfo(string address)
        {
            var ip = address.Split(':')[0];
            if (!ushort.TryParse(address.Split(':')[1], out var port))
            {
                await ReplyNewEmbed("Invalid port number");
                return;
            }

            await ReplyEmbed(SourceServerStatusService.GetEmbed(ip, port));
        }

        [Alias("mc")]
        [Command("minecraft")]
        [Summary("Minecraft server info")]
        public async Task Minecraft()
        {
            var builder = await SourceServerStatusService.GetMinecraftEmbed();
            await ReplyEmbed(builder);
        }


        [Alias("jj")]
        [Command("justjump")]
        [Summary("JustJust server info")]
        public async Task JustJumpInfo()
        {
            await ReplyEmbed(SourceServerStatusService.JustJumpEmbed);
        }

        [Alias("ht")]
        [Command("hightower")]
        [Summary("Hightower server info")]
        public async Task HighTowerInfo()
        {
            await ReplyEmbed(SourceServerStatusService.HightowerEmbed);
        }
    }
}