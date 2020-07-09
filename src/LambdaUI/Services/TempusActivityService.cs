using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Logging;
using LambdaUI.Models.Tempus;
using LambdaUI.Models.Tempus.Activity;
using LambdaUI.Models.Tempus.Responses;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    internal static class TempusActivityService
    {
        internal static Embed GetMapTopTimesEmbed(List<MapTop> topTimes)
        {
            try
            {
                var builder = new EmbedBuilder { Title = "**Map Top Times**" };
                var quickRecords = new MapTop[topTimes.Count];
                topTimes.CopyTo(quickRecords);
                var description = FormatTopTimes(topTimes.Take(TempusConstants.RecordPerPage));
                builder
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage}")
                    .WithCurrentTimestamp();
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        internal static Embed GetMapRecordsEmbed(List<MapWr> records)
        {
            try
            {
                var builder = new EmbedBuilder { Title = "**Map Records**" };
                var quickRecords = new MapWr[records.Count];
                records.CopyTo(quickRecords);
                var description = FormatRecords(records.Take(TempusConstants.RecordPerPage));
                builder
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage}")
                    .WithCurrentTimestamp(); return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        internal static Embed GetCourseRecordsEmbed(List<CourseWr> records)
        {
            try
            {
                var builder = new EmbedBuilder { Title = "**Course Records**" };
                var quickRecords = new CourseWr[records.Count];
                records.CopyTo(quickRecords);
                var description = FormatCourseRecords(records.Take(TempusConstants.RecordPerPage));
                builder
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage}")
                    .WithCurrentTimestamp();
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        internal static Embed GetBonusRecordsEmbed(List<BonusWr> records)
        {
            try
            {
                var builder = new EmbedBuilder { Title = "**Bonus Records**" };
                var quickRecords = new BonusWr[records.Count];
                records.CopyTo(quickRecords);
                var description = FormatBonusRecords(records.Take(TempusConstants.RecordPerPage));
                builder
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage}")
                    .WithCurrentTimestamp();
                return builder.Build();
            }
            catch (Exception e)
            {
                return Logger.LogException(e);
            }
        }

        private static string FormattedDuration(double duration)
        {
            var seconds = (int)Math.Truncate(duration);
                var milliseconds = (duration - (int)Math.Truncate(duration)) * 1000;
                var timespan = new TimeSpan(0, 0, 0, seconds, (int)Math.Truncate(milliseconds));
                return timespan.Days > 0 ? timespan.ToString(@"dd\:hh\:mm\:ss\.ff") : timespan.ToString(timespan.Hours > 0 ? @"hh\:mm\:ss\.ff" : @"mm\:ss\.ff");
        }

        private static string FormatRecords(IEnumerable<MapWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClassEmote(nextItem.RecordInfo.Class)}WR" 
                                         + FormatRecordSuffix(nextItem.MapInfo, nextItem.RecordInfo, nextItem.PlayerInfo));

        private static string FormatTopTimes(IEnumerable<MapTop> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClassEmote(nextItem.RecordInfo.Class)} #{nextItem.Rank}" 
                                         + FormatRecordSuffix(nextItem.MapInfo, nextItem.RecordInfo, nextItem.PlayerInfo));

        private static string FormatCourseRecords(IEnumerable<CourseWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClassEmote(nextItem.RecordInfo.Class)} C{nextItem.ZoneInfo.Zoneindex}" 
                                         + FormatRecordSuffix(nextItem.MapInfo, nextItem.RecordInfo, nextItem.PlayerInfo));

        private static string FormatBonusRecords(IEnumerable<BonusWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClassEmote(nextItem.RecordInfo.Class)} B{nextItem.ZoneInfo.Zoneindex}"
                                         + FormatRecordSuffix(nextItem.MapInfo, nextItem.RecordInfo, nextItem.PlayerInfo));

        public static string FormatRecordSuffix(MapFullOverviewModel mapOverview, RecordModel record) =>
            FormatRecordSuffix(new MapInfo {Id = mapOverview.MapInfo.Id, Name = mapOverview.MapInfo.Name, DateAdded = mapOverview.MapInfo.DateAdded},
                new RecordInfoShort {Duration = record.Duration, Id = record.Id},
                new PlayerInfo {Id = record.Id, Name = record.Name, Steamid = record.SteamId});
        public static string FormatRecordSuffix(MapInfo mapInfo, RecordInfoShort recordInfo, PlayerInfo playerInfo) =>
            $" | {DiscordHelper.FormatUrlMarkdown(mapInfo.Name.EscapeDiscordChars(), TempusHelper.GetMapUrl(mapInfo.Name))} | {DiscordHelper.FormatUrlMarkdown($"**{FormattedDuration(recordInfo.Duration).EscapeDiscordChars()}**", TempusHelper.GetRecordUrl(recordInfo.Id))} | {DiscordHelper.FormatUrlMarkdown(playerInfo.Name.EscapeDiscordChars(), TempusHelper.GetPlayerUrl(playerInfo.Id))}" +
                Environment.NewLine;
    }
}