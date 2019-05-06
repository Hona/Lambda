using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaUI.Utilities
{
    public static class DiscordHelper
    {
        public static string FormatUrlMarkdown(string text, string url) =>
            $"[{text}]({url})";
    }
}
