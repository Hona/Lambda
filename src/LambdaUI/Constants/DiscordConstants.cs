using System;
using System.IO;

namespace LambdaUI.Constants
{
    internal static class DiscordConstants
    {
        internal const char CommandPrefix = '!';

        internal const int LogPaddingLength = 10;

        internal static readonly string DatabaseInfoPath =
            $"{CurrentDirectory}config/database.txt";

        internal static readonly string TokenPath =
            $"{CurrentDirectory}config/token.txt";

        internal static readonly string LogFilePath =
            $"{CurrentDirectory}config/log.txt";

        private static string CurrentDirectory => $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}";
        
    }
}
