using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Data.Access;
using LambdaUI.Logging;
using LambdaUI.Models.Tempus.DetailedMapList;
using LambdaUI.Models.Tempus.Rank;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    public static class TempusApiService
    {
        public static Embed CachedStalkTopEmbed { get; private set; }

        public static async Task<Embed> UpdateStalkTopEmbedAsync(TempusDataAccess tempusDataAccess)
        {
            try
            {
                var servers = (await tempusDataAccess.GetServerStatusAsync()).Where(x => x != null).ToArray();
                var users = servers.Where(x => x.GameInfo != null &&
                                               (x.GameInfo != null || x.ServerInfo != null ||
                                                x.GameInfo.Users != null) &&
                                               x.GameInfo.Users.Count != 0)
                    .SelectMany(x => x.GameInfo.Users).Where(x => x?.Id != null).ToArray();

                var userIdStrings = (from user in users where user?.Id != null select user.Id.ToString()).ToList();

                var rankTasks = new List<Task<Rank>>();
                rankTasks.AddRange(userIdStrings.Select(tempusDataAccess.GetUserRankAsync));


                var ranks = await Task.WhenAll(rankTasks);
                var rankedUsers = ranks.ToDictionary(rank => users.First(x => x.Id == rank.PlayerInfo.Id), rank =>
                    rank.ClassRankInfo.DemoRank.Rank <= rank.ClassRankInfo.SoldierRank.Rank
                        ? rank.ClassRankInfo.DemoRank.Rank
                        : rank.ClassRankInfo.SoldierRank.Rank);

                var output = rankedUsers.OrderBy(x => x.Value).Take(7);
                var rankedLines = "";
                foreach (var (key, value) in output)
                {
                    if (key == null || value > 100) continue;
                    var server = servers
                        .FirstOrDefault(x =>
                            x.GameInfo?.Users != null &&
                            x.GameInfo.Users.Count(z => z.Id.HasValue && z.Id == key.Id) != 0);
                    if (server == null || key.Id == null) continue;
                    rankedLines +=
                        $"Rank {value} - {DiscordHelper.FormatUrlMarkdown(key.Name.EscapeDiscordChars(), TempusHelper.GetPlayerUrl(key.Id.Value))} on {DiscordHelper.FormatUrlMarkdown(server.GameInfo.CurrentMap.EscapeDiscordChars(), TempusHelper.GetMapUrl(server.GameInfo.CurrentMap))} {DiscordHelper.FormatUrlMarkdown(server.ServerInfo.Shortname, TempusHelper.GetServerUrl(server.ServerInfo.Id))}{Environment.NewLine}";
                }

                var builder =
                    new EmbedBuilder {Title = "**Highest Ranked Players Online** (Top 100)", Description = rankedLines}
                        .WithFooter(DateTimeHelper.ShortDateTimeNowString).WithColor(ColorConstants.InfoColor);
                CachedStalkTopEmbed = builder.Build();
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        public static async Task<Embed> GetStalkTopEmbedAsync(TempusDataAccess tempusDataAccess)
        {
            try
            {
                if (CachedStalkTopEmbed == null) return await UpdateStalkTopEmbedAsync(tempusDataAccess);
                return CachedStalkTopEmbed;
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        public static Embed GetMapInfoEmbed(DetailedMapOverviewModel map)
        {
            try
            {
                var builder = new EmbedBuilder();
                builder.WithTitle(map.Name);
                var tierText = $"Solly : T{map.TierInfo.Soldier}";
                if (!string.IsNullOrWhiteSpace(map.Videos.Soldier))
                    tierText += $" [Showcase]({TempusHelper.GetYoutubeUrl(map.Videos.Soldier)})";
                tierText += $" | Demo : T{map.TierInfo.Demoman} ";
                if (!string.IsNullOrWhiteSpace(map.Videos.Demoman))
                    tierText += $"[Showcase]({TempusHelper.GetYoutubeUrl(map.Videos.Demoman)})";
                builder.WithDescription(tierText + Environment.NewLine +
                                        DiscordHelper.FormatUrlMarkdown("Tempus.xyz",
                                            TempusHelper.GetMapUrl(map.Name)));
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }
    }
}