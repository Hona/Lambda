using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    public static class DiscordService
    {
        private static string GetSummaryString(string summary) => string.IsNullOrEmpty(summary) ? "" : $"({summary})";

        public static IEnumerable<Embed> GetRoleEmbeds(string roleParam, IGuild guild)
        {
            var roles = guild.Roles.Where(x => x.Name.ToLower().Contains(roleParam.ToLower())).ToArray();
            if (!roles.Any())
            {
                yield return EmbedHelper.CreateEmbed("No roles found from name", false);
                yield break;
            }
            foreach (var role in roles.Take(5))
            {
                var builder = new EmbedBuilder { Title = "@" + role.Name };
                builder.AddField("Separate from @everyone", role.IsHoisted);
                builder.AddField("Mentionable", role.IsMentionable);
                builder.AddField("Hierarchical position", role.Position);
                builder.AddField("Guild", role.Guild);
                builder.AddField("Created at", role.CreatedAt.ToString("d"));
                builder.AddField("Permissions", PermissionsToString(role.Permissions));
                builder.WithColor(role.Color);
                builder.WithFooter(role.Id.ToString());
                yield return builder.Build();
            }
        }
        public static Embed GetServerEmbed(IGuild guild)
        {
            if (guild is SocketGuild server)
            {
                var builder = new EmbedBuilder { Title = server.Name };
                builder.AddField("Channels", server.Channels.Count)
                    .AddField("Created", server.CreatedAt.ToString("d"))
                    .AddField("Default Notifications", server.DefaultMessageNotifications.ToString())
                    .AddField("Members", server.MemberCount)
                    .AddField("2FA", server.MfaLevel.ToString())
                    .AddField("Owner", server.Owner.Username)
                    .WithColor(ColorConstants.InfoColor)
                    .WithFooter(server.Id.ToString());
                return builder.Build();
            }
            return EmbedHelper.CreateEmbed("Guild is not a SocketGuild", false);
        }

        public static async Task<Embed> GetChannelEmbedAsync(IChannel channel)
        {

           var builder = new EmbedBuilder {Title = "Channel: " + channel.Name}
           .AddField("Users", (await channel.GetUsersAsync().FlattenAsync()).Count())
           .AddField("Created At", channel.CreatedAt.ToString("g"))
           .WithFooter(channel.Id.ToString());
            return builder.Build();

        }

        public static Embed GetGuildUserEmbed(SocketGuildUser user)
        {
            var builder = new EmbedBuilder { Title = "@" + user.Username };
            builder.AddField("Username",
                user.Username + "#" + user.DiscriminatorValue + NicknameString(user.Nickname));
            builder.AddField("Bot", user.IsBot);
            if (user.JoinedAt != null)
                builder.AddField("Joined Server", user.JoinedAt.Value.ToString("d"));
            builder.AddField("Created Account", user.CreatedAt.ToString("d"));
            if (user.Activity != null) builder.AddField("Game", user.Activity.Name);
            builder.AddField("Roles",
                user.Roles.Aggregate("", (currentString, nextRole) => currentString + nextRole.Mention + ", "));
            builder.AddField("Permissions", PermissionsToString(user.GuildPermissions));
            if (user.Hierarchy == int.MaxValue)
            {
                builder.WithColor(user.Guild.Roles.OrderByDescending(x => x.Position).First().Color);
            }
            else
            {
                var role = user.Guild.Roles.First(x => x.Position == user.Hierarchy);
                if (role != null) builder.WithColor(role.Color);
            }

            builder.WithFooter(user.Id.ToString());
            return builder.Build();
        }
        public static Embed GetUserEmbed(RestUser user)
        {
            var builder = new EmbedBuilder { Title = "@" + user.Username };
            builder.AddField("Username",
                user.Username + "#" + user.DiscriminatorValue);
            builder.AddField("Bot", user.IsBot);

            builder.AddField("Created Account", user.CreatedAt.ToString("d"));
            if (user.Activity != null) builder.AddField("Game", user.Activity.Name);


            builder.WithFooter(user.Id.ToString());
            return builder.Build();
        }

        public static Embed GetHelpEmbed(string moduleName, CommandService commandService, ICommandContext context)
        {
            return moduleName == "" ? GetModulesOverviewEmbed(commandService, context) : GetModuleHelpEmbed(commandService.Modules.First(x => x.Name.ToLower().Contains(moduleName.ToLower())), context);
        }

        private static Embed GetModuleHelpEmbed(ModuleInfo module, ICommandContext context)
        {
            var title = $"Help: **({module.Name})**";
            var text = module.Commands.Where(x => x.CheckPreconditionsAsync(context).GetAwaiter().GetResult().IsSuccess).Aggregate("",
                (current, command) =>
                    current +
                    $"**__{DiscordConstants.CommandPrefix + command.Name}__**{Environment.NewLine}**{command.Summary}**. Parameters: {command.Parameters.Aggregate("", (currentString, nextParameter) => currentString + $"{nextParameter.Name} {GetSummaryString(nextParameter.Summary)}, ").TrimEnd(' ', ',')}{Environment.NewLine}");
            return EmbedHelper.CreateEmbed(title, text, false);
        }
        private static Embed GetModulesOverviewEmbed(CommandService commandService, ICommandContext context)
        {
            var title = $"Per module help commands: ({DiscordConstants.CommandPrefix}help [module])";
            var text = commandService.Modules.Where(x => !x.Name.Contains("ModuleBase")).Aggregate("",
                (current, module) =>
                    current +
                    $"**{module.Name}** ({module.Summary}), {module.Commands.Count} command/s{Environment.NewLine}  ");
            return EmbedHelper.CreateEmbed(title, text, false);
        }
        public static async Task<Embed> GetDiscordObjectEmbedAsync(DiscordSocketClient client, ulong id)
        {
            
            var guild = client.GetGuild(id);
            if ( guild != null)
            {
                return GetServerEmbed(guild);
            }

            var channel = client.GetChannel(id);
            if (channel != null)
            {
                return await GetChannelEmbedAsync(channel);
            }

            var user = await client.Rest.GetUserAsync(id);
            if (user != null)
            {
                return GetUserEmbed(user);
            }

            return EmbedHelper.CreateEmbed("No global discord object found", false);


        }
        private static string NicknameString(string nickname) => string.IsNullOrWhiteSpace(nickname)
            ? string.Empty
            : $" ({nickname})";

        private static string PermissionsToString(GuildPermissions perms) => perms.ToList()
            .Aggregate("", (currentString, nextPermission) => currentString + nextPermission.ToString() + ", ")
            .TrimEnd(' ', ',');
    }
}
