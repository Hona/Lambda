using System;
using System.Threading.Tasks;
using Discord;
using LambdaUI.Constants;

namespace LambdaUI.Logging
{
    internal static class Logger
    {
        internal static void LogInfo(string source, string message) => Log(new LogMessage(LogSeverity.Info, source, message));

        internal static void LogError(string source, string message) => Log(new LogMessage(LogSeverity.Error, source, message));

        internal static void LogWarning(string source, string message) => Log(new LogMessage(LogSeverity.Warning, source, message));

        internal static Task Log(LogMessage logMessage)
        {
            Console.WriteLine(
                $"{logMessage.Severity.ToString().PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Source.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Message.PadRight(DiscordConstants.LogPaddingLength)}    {logMessage.Exception}");
            return Task.CompletedTask;
        }
    }
}