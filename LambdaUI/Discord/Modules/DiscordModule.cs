using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Constants;

namespace LambdaUI.Discord.Modules
{
    public class DiscordModule : ExtraModuleBase
    {
        [Command("roleinfo")]
        public async Task GetRoleInfo(string roleParam)
        {
            var roles = Context.Guild.Roles.Where(x => x.Name.ToLower().Contains(roleParam.ToLower()));
            foreach (var role in roles.Take(5))
            {
                var builder = new EmbedBuilder {Title = "@" + role.Name};
                builder.AddField("Separate from @everyone", role.IsHoisted);
                builder.AddField("Mentionable", role.IsMentionable);
                builder.AddField("Hierarchical position", role.Position);
                builder.AddField("Guild", role.Guild);
                builder.AddField("Created at", role.CreatedAt.ToString("d"));
                builder.AddField("Permissions", PermissionsToString(role.Permissions));
                builder.WithColor(role.Color);
                builder.WithFooter(role.Id.ToString());
                await ReplyEmbed(builder);
            }
        }

        [Command("userinfo")]
        public async Task GetUserInfo(SocketGuildUser userParam)
        {
            var builder = new EmbedBuilder {Title = "@" + userParam.Username};
            builder.AddField("Username",
                userParam.Username + "#" + userParam.DiscriminatorValue + NicknameString(userParam.Nickname));
            builder.AddField("Bot", userParam.IsBot);
            if (userParam.JoinedAt != null)
                builder.AddField("Joined Server", userParam.JoinedAt.Value.ToString("d"));
            builder.AddField("Created Account", userParam.CreatedAt.ToString("d"));
            if (userParam.Activity != null) builder.AddField("Game", userParam.Activity.Name);
            builder.AddField("Roles",
                userParam.Roles.Aggregate("", (currentString, nextRole) => currentString + nextRole.Mention + ", "));
            builder.AddField("Permissions", PermissionsToString(userParam.GuildPermissions));
            if (userParam.Hierarchy == int.MaxValue)
            {
                builder.WithColor(Context.Guild.Roles.OrderByDescending(x => x.Position).First().Color);
            }
            else
            {
                var role = Context.Guild.Roles.First(x => x.Position == userParam.Hierarchy);
                if (role != null) builder.WithColor(role.Color);
            }

            builder.WithFooter(userParam.Id.ToString());
            await ReplyEmbed(builder);
        }

        [Command("serverinfo")]
        public async Task GetServerInfo()
        {
            if (Context.Guild is SocketGuild server)
            {
                var builder = new EmbedBuilder {Title = server.Name};
                builder.AddField("Channels", server.Channels.Count)
                    .AddField("Created", server.CreatedAt.ToString("d"))
                    .AddField("Default Notifications", server.DefaultMessageNotifications.ToString())
                    .AddField("Members", server.MemberCount)
                    .AddField("2FA", server.MfaLevel.ToString())
                    .AddField("Owner", server.Owner.Username)
                    .WithColor(ColorConstants.InfoColor)
                    .WithFooter(server.Id.ToString());
                await ReplyEmbed(builder);
            }
        }

        private static string NicknameString(string nickname) => string.IsNullOrWhiteSpace(nickname)
            ? string.Empty
            : $" ({nickname})";

        private static string PermissionsToString(GuildPermissions perms) => perms.ToList()
            .Aggregate("", (currentString, nextPermission) => currentString + nextPermission.ToString() + ", ")
            .TrimEnd(' ', ',');
    }
}