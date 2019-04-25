using System;
using System.IO;

namespace LambdaUI.Constants
{
    internal static class DiscordConstants
    {
        internal const char CommandPrefix = '!';

        internal const int LogPaddingLength = 10;

        internal static readonly string DatabaseInfoPath =
            $"{CurrentDirectory}database.txt";

        internal static readonly string TokenPath =
            $"{CurrentDirectory}token.txt";

        internal static readonly string LogFilePath =
            $"{CurrentDirectory}log.txt";

        private static string CurrentDirectory => $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}";
        
    }
}