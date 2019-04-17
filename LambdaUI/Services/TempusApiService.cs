using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Data;
using LambdaUI.Models.Tempus;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    public static class TempusApiService
    {
        public static async Task SendStalkTopEmbedAsync(TempusDataAccess tempusDataAccess, IMessageChannel channel)
        {
            var servers = (await tempusDataAccess.GetServerStatusAsync()).Where(x => x != null).ToArray();
            var users = servers.Where(x => x.GameInfo != null &&
                                           (x.GameInfo != null || x.ServerInfo != null || x.GameInfo.Users != null) &&
                                           x.GameInfo.Users.Count != 0)
                .SelectMany(x => x.GameInfo.Users).ToArray();
            var rankedUsers = new Dictionary<ServerPlayerModel, int>();

            foreach (var user in users)
            {
                if (user?.Id == null) continue;
                var rank = await tempusDataAccess.GetUserRank(user.Id.ToString());
                rankedUsers.Add(user,
                    rank.ClassRankInfo.DemoRank.Rank >= rank.ClassRankInfo.SoldierRank.Rank
                        ? rank.ClassRankInfo.DemoRank.Rank
                        : rank.ClassRankInfo.SoldierRank.Rank);
            }
            var output = rankedUsers.OrderBy(x => x.Value);
            var rankedLines = "";
            foreach (var pair in output)
            {
                if (pair.Key == null) continue;
                var serverString = servers
                    .First(x => x.GameInfo?.Users.Count(z => z.Id.HasValue && z.Id == pair.Key.Id) != 0)
                    .ServerInfo?.Name;
                rankedLines +=
                    $"Rank {pair.Value} - {pair.Key.Name.EscapeDiscordChars()} on {serverString}{Environment.NewLine}";
            }

            await channel.SendMessageAsync(
                embed: EmbedHelper.CreateEmbed("**Highest Ranked Players Online**", rankedLines, false));
        }
    }
}