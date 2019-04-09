using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LambdaUI
{
    internal static class Constants
    {
        private static string CurrentDirectory => $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}";
        internal static readonly string DatabaseInfoPath =
            $"{CurrentDirectory}database.txt";
        internal static readonly string TokenPath =
            $"{CurrentDirectory}token.txt";

        internal const char CommandPrefix = '!';
    }
}
