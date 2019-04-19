using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Data;
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

    }
}
