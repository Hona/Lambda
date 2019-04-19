using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Data;
using LambdaUI.Models.Simply;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    public class SimplyDataService
    {
        private readonly SimplyHightowerDataAccess _simplyHightowerDataAccess;
        private readonly JustJumpDataAccess _justJumpDataAccess;
        public SimplyDataService(SimplyHightowerDataAccess simplyHightowerDataAccess, JustJumpDataAccess justJumpDataAccess)
        {
            _simplyHightowerDataAccess = simplyHightowerDataAccess;
            _justJumpDataAccess = justJumpDataAccess;
        }

        public async Task<Embed> GetHightowerRankEmbedAsync()
        {
            var topPlayers = await _simplyHightowerDataAccess.GetTopHightowerRank(15);
            var topHightowerScoreString = "";

            for (var i = 0; i < topPlayers.Count; i++)
                topHightowerScoreString +=
                    $"**__#{i + 1}__** | **__{topPlayers[i].Nickname}__** {Math.Round(topPlayers[i].Points)} points, **{topPlayers[i].Kills} kills**, {topPlayers[i].Deaths} deaths, **{Math.Round((double)topPlayers[i].Kills / topPlayers[i].Deaths, 1)} K/D**, {topPlayers[i].Headshots} headshots, **{Math.Round((decimal)topPlayers[i].PlayTime / 60 / 60)} hours**{Environment.NewLine}";

            var builder = new EmbedBuilder { Title = "**Top Ranked Hightower Players**" };

            builder.WithDescription(topHightowerScoreString)
                .WithColor(ColorConstants.InfoColor)
                .WithFooter("Updated " + DateTime.Now.ToShortTimeString());
            return builder.Build();
        }
        public async Task<Embed> GetRecentRecordEmbedAsync()
        {
            var recentRecords = await _justJumpDataAccess.GetRecentRecords(10);
            var recentRecordsString = recentRecords.Aggregate("",
                (currentString, nextHighscore) => currentString +
                                                  $"{SimplyHelper.ClassToShortString(nextHighscore.Class)} | **__{TempusHelper.TimeSpanToFormattedTime(SimplyHelper.GetTimeSpan(nextHighscore.RunTime))}__** | **{nextHighscore.Map}** | **{nextHighscore.Name}**" +
                                                  Environment.NewLine);

            var builder = new EmbedBuilder { Title = "**Recent Map Records**" };

            builder.WithDescription(recentRecordsString)
                .WithColor(ColorConstants.InfoColor)
                .WithFooter("Updated " + DateTime.Now.ToShortTimeString());
            return builder.Build();
        }
        public async Task<Embed> GetTopPlayersEmbedAsync()
        {
            var overallTopString = await GetOverallTopString();
            var soldierTopString = await GetSoldierTopString();
            var demomanTopString = await GetDemomanTopString();
            var concTopString = await GetConcTopString();
            var engiTopString = await GetEngiTopString();
            var pyroTopString = await GetPyroTopString();

            var builder = new EmbedBuilder { Title = "**Top Ranked Jumpers**" };
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
        private async Task<string> GetPyroTopString()
        {
            var pyroTop = await _justJumpDataAccess.GetTopPyro(SimplyConstants.TopRankCount);
            return TopRankToString(pyroTop, "Pyro");
        }

        private async Task<string> GetEngiTopString()
        {
            var engiTop = await _justJumpDataAccess.GetTopEngi(SimplyConstants.TopRankCount);
            return TopRankToString(engiTop, "Engi");
        }

        private async Task<string> GetConcTopString()
        {
            var concTop = await _justJumpDataAccess.GetTopConc(SimplyConstants.TopRankCount);
            return TopRankToString(concTop, "Conc");
        }

        private async Task<string> GetDemomanTopString()
        {
            var demoTop = await _justJumpDataAccess.GetTopDemo(SimplyConstants.TopRankCount);
            return TopRankToString(demoTop, "Demoman");
        }

        private async Task<string> GetOverallTopString()
        {
            var overallTop = await _justJumpDataAccess.GetTopOverall(SimplyConstants.TopRankCount);
            return TopRankToString(overallTop, "Overall");
        }

        private async Task<string> GetSoldierTopString()
        {
            var soldierTop = await _justJumpDataAccess.GetTopSolly(SimplyConstants.TopRankCount);
            return TopRankToString(soldierTop, "Soldier");
        }

        private string TopRankToString(List<JumpRankModel> list, string property)
        {
            var outputString = "";
            for (var i = 0; i < list.Count; i++) outputString += FormatLine(list, i, property);
            return outputString;
        }

        private string FormatLine(List<JumpRankModel> list, int index, string property)
        {
            return
                $"**#{index + 1}**: **{list[index].Name}** - {GetPropValue(list[index], property)} points{Environment.NewLine}";
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
