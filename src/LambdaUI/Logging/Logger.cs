using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Utilities;

namespace LambdaUI.Logging
{
    internal static class Logger
    {
        private static bool _logToChannel;
        private static DiscordSocketClient _client;
        private static ITextChannel _channel;

        public static void StartLoggingToChannel(DiscordSocketClient client, ITextChannel channel)
        {
            _client = client;
            _channel = channel;
            _logToChannel = true;
        }

        public static void StopLoggingToChannel()
        {
            _logToChannel = false;
        }

        internal static void LogInfo(string source, string message) => Log(new LogMessage(LogSeverity.Info, source,
            message));

        internal static Embed LogException(Exception e)
        {
            var logMessage = new LogMessage(LogSeverity.Error,
                new StackTrace(e, true).GetFrame(0).GetMethod().ReflectedType?.FullName,
                e.ToString());
            Log(logMessage);
            return GetLogEmbed(logMessage);
        }

        internal static void LogError(string source, string message) => Log(new LogMessage(LogSeverity.Error, source,
            message));

        internal static void LogWarning(string source, string message) => Log(new LogMessage(LogSeverity.Warning,
            source, message));

        internal static Task Log(LogMessage logMessage)
        {
            if (logMessage.Message == null)
                logMessage = new LogMessage(logMessage.Severity, logMessage.Source, "", logMessage.Exception);
            if (logMessage.Source == null)
                logMessage = new LogMessage(logMessage.Severity, "", logMessage.Message, logMessage.Exception);
            if (_logToChannel && logMessage.Severity == LogSeverity.Error || logMessage.Severity == LogSeverity.Critical || logMessage.Severity == LogSeverity.Warning)
            {
                var embed = GetLogEmbed(logMessage);

                _channel.SendMessageAsync(embed: embed);
            }
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ColorConstants.ErrorLogColor;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ColorConstants.WarningLogColor;
                    break;
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ColorConstants.InfoLogColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine(
                $"{logMessage.Severity.ToString().PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Source.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Message.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Exception}");
            LogToFileAsync(logMessage);
            Console.ForegroundColor = ColorConstants.InfoLogColor;
            return Task.CompletedTask;
        }

        private static string FormatLogMessage(LogMessage logMessage) =>
            $"{logMessage.Severity.ToString().PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Source.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Message.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Exception}";
        private static Embed GetLogEmbed(LogMessage logMessage)
        {
            Color color;
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    color = Color.Red;
                    break;
                case LogSeverity.Warning:
                    color = Color.Orange;
                    break;
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    color = Color.Blue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var embed = new EmbedBuilder().WithDescription($"**{logMessage.Source}**" + Environment.NewLine + logMessage.Message).WithColor(color)
                .WithFooter(DateTimeHelper.ShortDateTimeNowString);
            if (logMessage.Exception != null)
                embed.AddField("Exception", logMessage.Exception);
            return embed.Build();
        }

        private static async void LogToFileAsync(LogMessage logMessage)
        {
            try
            {
                await File.AppendAllTextAsync(DiscordConstants.LogFilePath, FormatLogMessage(logMessage));
            }
            catch (Exception e)
            {
                // TODO IGNORE?
            }
        }
    }
}