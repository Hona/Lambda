using System;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;

namespace LambdaUI.Logging
{
    internal static class Logger
    {
        internal static void LogInfo(string source, string message) => Log(new LogMessage(LogSeverity.Info, source,
            message));

        internal static void LogError(string source, string message) => Log(new LogMessage(LogSeverity.Error, source,
            message));

        internal static void LogWarning(string source, string message) => Log(new LogMessage(LogSeverity.Warning,
            source, message));

        internal static Task Log(LogMessage logMessage)
        {
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
            if (logMessage.Message == null)
                logMessage = new LogMessage(logMessage.Severity, logMessage.Source, "", logMessage.Exception);
            if (logMessage.Source == null)
                logMessage = new LogMessage(logMessage.Severity, "", logMessage.Message, logMessage.Exception);
            Console.WriteLine(
                $"{logMessage.Severity.ToString().PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Source.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Message.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Exception}");

            Console.ForegroundColor = ColorConstants.InfoLogColor;
            return Task.CompletedTask;
        }
    }
}