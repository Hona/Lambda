using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;
using LambdaUI.Models.Tempus.Activity;
using LambdaUI.Utilities;

namespace LambdaUI.Services
{
    internal static class TempusUpdaterService
    {
        internal static async Task SendMapTopTimes(List<MapTop> topTimes, IMessageChannel channel)
        {
            var builder = new EmbedBuilder {Title = "**Map Top Times**"};
            var quickRecords = new MapTop[topTimes.Count];
            topTimes.CopyTo(quickRecords);
            var description = FormatTopTimes(topTimes.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        internal static async Task SendMapRecords(List<MapWr> records, IMessageChannel channel)
        {
            var builder = new EmbedBuilder {Title = "**Map Records**"};
            var quickRecords = new MapWr[records.Count];
            records.CopyTo(quickRecords);
            var description = FormatRecords(records.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        internal static async Task SendCourseRecords(List<CourseWr> records, IMessageChannel channel)
        {
            var builder = new EmbedBuilder {Title = "**Course Records**"};
            var quickRecords = new CourseWr[records.Count];
            records.CopyTo(quickRecords);
            var description = FormatCourseRecords(records.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        internal static async Task SendBonusRecords(List<BonusWr> records, IMessageChannel channel)
        {
            var builder = new EmbedBuilder {Title = "**Bonus Records**"};
            var quickRecords = new BonusWr[records.Count];
            records.CopyTo(quickRecords);
            var description = FormatBonusRecords(records.Take(TempusConstants.RecordPerPage));
            builder.WithDescription(description).WithColor(Color.Blue)
                .WithFooter($"Showing records 1-{TempusConstants.RecordPerPage} | {DateTime.Now:t}");
            await channel.SendMessageAsync(embed: builder.Build());
        }

        private static string FormattedDuration(double duration) => new TimeSpan(0, 0, (int) Math.Truncate(duration),
            (int) (duration - (int) Math.Truncate(duration))).ToString("c");

        private static string FormatRecords(IEnumerable<MapWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClass(nextItem.RecordInfo.Class)}WR | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);

        private static string FormatTopTimes(IEnumerable<MapTop> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClass(nextItem.RecordInfo.Class)} #{nextItem.RecordInfo.Rank} | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);

        private static string FormatCourseRecords(IEnumerable<CourseWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClass(nextItem.RecordInfo.Class)} C{nextItem.ZoneInfo.Zoneindex} | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);

        private static string FormatBonusRecords(IEnumerable<BonusWr> records) => records.Aggregate("",
            (currentString, nextItem) => currentString +
                                         $"{TempusHelper.GetClass(nextItem.RecordInfo.Class)} B{nextItem.ZoneInfo.Zoneindex} | [{nextItem.MapInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/maps/{nextItem.MapInfo.Name}) | [**{FormattedDuration(nextItem.RecordInfo.Duration).EscapeDiscordChars()}**](https://tempus.xyz/records/{nextItem.RecordInfo.Id}) | [{nextItem.PlayerInfo.Name.EscapeDiscordChars()}](https://tempus.xyz/players/{nextItem.PlayerInfo.Id})" +
                                         Environment.NewLine);
    }
}