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
        public async Task ServerInfoAsync(string address)
        {
            var ip = address.Split(':')[0];
            if (!ushort.TryParse(address.Split(':')[1], out var port))
            {
                await ReplyNewEmbedAsync("Invalid port number");
                return;
            }

            await ReplyEmbedAsync(SourceServerStatusService.GetEmbed(ip, port));
        }

        [Alias("mc")]
        [Command("minecraft")]
        [Summary("Minecraft server info")]
        public async Task MinecraftAsync()
        {
            var builder = await SourceServerStatusService.GetMinecraftEmbedAsync();
            await ReplyEmbedAsync(builder);
        }


        [Alias("jj")]
        [Command("justjump")]
        [Summary("JustJust server info")]
        public async Task JustJumpInfoAsync()
        {
            await ReplyEmbedAsync(SourceServerStatusService.JustJumpEmbed);
        }

        [Alias("ht")]
        [Command("hightower")]
        [Summary("Hightower server info")]
        public async Task HighTowerInfoAsync()
        {
            await ReplyEmbedAsync(SourceServerStatusService.HightowerEmbed);
        }
    }
}