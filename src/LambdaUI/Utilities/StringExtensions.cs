using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaUI.Utilities
{
    internal static class StringExtensions
    {
        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static string EscapeDiscordChars(this string s)
        {
            var chars = new List<char> {'*', '_', '~', '`', '@'};
            return chars.Aggregate(s, (current, character) => current.Replace($"{character}", $@"\{character}"));
        }
    }
}