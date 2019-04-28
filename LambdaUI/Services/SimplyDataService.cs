using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Data.Access.Simply;
using LambdaUI.Logging;
using LambdaUI.Models.Simply;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    public class SimplyDataService
    {
        private readonly JustJumpDataAccess _justJumpDataAccess;
        private readonly SimplyHightowerDataAccess _simplyHightowerDataAccess;

        public SimplyDataService(SimplyHightowerDataAccess simplyHightowerDataAccess,
            JustJumpDataAccess justJumpDataAccess)
        {
            _simplyHightowerDataAccess = simplyHightowerDataAccess;
            _justJumpDataAccess = justJumpDataAccess;
        }

        public async Task<Embed> GetHightowerRankEmbedAsync()
        {
            try
            {
                var topPlayers = await _simplyHightowerDataAccess.GetTopHightowerRankAsync(15);
                var topHightowerScoreString = "";

                for (var i = 0; i < topPlayers.Count; i++)
                    topHightowerScoreString +=
                        $"**__#{i + 1}__** | **__{topPlayers[i].Nickname}__** {Math.Round(topPlayers[i].Points)} points, **{topPlayers[i].Kills} kills**, {topPlayers[i].Deaths} deaths, **{Math.Round((double) topPlayers[i].Kills / topPlayers[i].Deaths, 1)} K/D**, {topPlayers[i].Headshots} headshots, **{Math.Round((decimal) topPlayers[i].PlayTime / 60 / 60)} hours**{Environment.NewLine}";

                var builder = new EmbedBuilder {Title = "**Top Ranked Hightower Players**"};

                builder.WithDescription(topHightowerScoreString)
                    .WithColor(ColorConstants.InfoColor)
                    .WithFooter("Updated " + DateTime.Now.ToShortTimeString());

                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        public async Task<Embed> GetRecentRecordEmbedAsync()
        {
            try
            {
                var recentRecords = await _justJumpDataAccess.GetRecentRecordsAsync(10);
                var recentRecordsString = recentRecords.Aggregate("",
                    (currentString, nextHighscore) => currentString +
                                                      $"{SimplyHelper.ClassToShortString(nextHighscore.Class)} | **__{TempusHelper.TimeSpanToFormattedTime(SimplyHelper.GetTimeSpan(nextHighscore.RunTime))}__** | **{nextHighscore.Map}** | **{nextHighscore.Name}**" +
                                                      Environment.NewLine);

                var builder = new EmbedBuilder {Title = "**Recent Map Records**"};

                builder.WithDescription(recentRecordsString)
                    .WithColor(ColorConstants.InfoColor)
                    .WithFooter("Updated " + DateTime.Now.ToShortTimeString());
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        public async Task<Embed> GetTopPlayersEmbedAsync()
        {
            try
            {
                var overallTopString = await GetOverallTopStringAsync();
                var soldierTopString = await GetSoldierTopStringAsync();
                var demomanTopString = await GetDemomanTopStringAsync();
                var concTopString = await GetConcTopStringAsync();
                var engiTopString = await GetEngiTopStringAsync();
                var pyroTopString = await GetPyroTopStringAsync();

                var builder = new EmbedBuilder {Title = "**Top Ranked Jumpers**"};
                builder.AddField("Overall", overallTopString)
                    .AddField("Soldier", soldierTopString)
                    .AddField("Demoman", demomanTopString)
                    .AddField("Engineer", engiTopString)
                    .AddField("Pyro", pyroTopString)
                    .AddField("Conc", concTopString)
                    .WithColor(ColorConstants.InfoColor)
                    .WithFooter("Updated " + DateTime.Now.ToShortTimeString());
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        private async Task<string> GetPyroTopStringAsync()
        {
            var pyroTop = await _justJumpDataAccess.GetTopPyroAsync(SimplyConstants.TopRankCount);
            return TopRankToString(pyroTop, "Pyro");
        }

        private async Task<string> GetEngiTopStringAsync()
        {
            var engiTop = await _justJumpDataAccess.GetTopEngiAsync(SimplyConstants.TopRankCount);
            return TopRankToString(engiTop, "Engi");
        }

        private async Task<string> GetConcTopStringAsync()
        {
            var concTop = await _justJumpDataAccess.GetTopConcAsync(SimplyConstants.TopRankCount);
            return TopRankToString(concTop, "Conc");
        }

        private async Task<string> GetDemomanTopStringAsync()
        {
            var demoTop = await _justJumpDataAccess.GetTopDemoAsync(SimplyConstants.TopRankCount);
            return TopRankToString(demoTop, "Demoman");
        }

        private async Task<string> GetOverallTopStringAsync()
        {
            var overallTop = await _justJumpDataAccess.GetTopOverallAsync(SimplyConstants.TopRankCount);
            return TopRankToString(overallTop, "Overall");
        }

        private async Task<string> GetSoldierTopStringAsync()
        {
            var soldierTop = await _justJumpDataAccess.GetTopSollyAsync(SimplyConstants.TopRankCount);
            return TopRankToString(soldierTop, "Soldier");
        }

        private static string TopRankToString(IReadOnlyList<JumpRankModel> list, string property)
        {
            var outputString = "";
            for (var i = 0; i < list.Count; i++) outputString += FormatLine(list, i, property);
            return outputString;
        }

        private static string FormatLine(IReadOnlyList<JumpRankModel> list, int index, string property) =>
            $"**#{index + 1}**: **{list[index].Name}** - {GetPropValue(list[index], property)} points{Environment.NewLine}";

        private static object GetPropValue(object src, string propName) => src.GetType().GetProperty(propName)
            .GetValue(src, null);
    }
}