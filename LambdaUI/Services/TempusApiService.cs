using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Data;
using LambdaUI.Data.Access;
using LambdaUI.Logging;
using LambdaUI.Models.Tempus;
using LambdaUI.Models.Tempus.DetailedMapList;
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

        public static Embed GetMapInfoEmbed(DetailedMapOverviewModel map)
        {
            try
            {
                var builder = new EmbedBuilder();
                builder.WithTitle($"[{map.Name}]({TempusHelper.GetMapUrl(map.Name)})");
                var teirText = $"Solly : T{map.TierInfo.Soldier}";
                if (!string.IsNullOrWhiteSpace(map.Videos.Soldier))
                {
                    teirText += $" [Showcase]({TempusHelper.GetYoutubeUrl(map.Videos.Soldier)})";
                }
                teirText += $" | Demo : T{map.TierInfo.Demoman} ";
                if (!string.IsNullOrWhiteSpace(map.Videos.Demoman))
                {
                    teirText += $"[Showcase]({TempusHelper.GetYoutubeUrl(map.Videos.Demoman)})";
                }
                builder.WithDescription(teirText);
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }


        }
    }
}