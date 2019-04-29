using System;
using Discord;
using LambdaUI.Constants;

namespace LambdaUI.Utilities
{
    internal static class EmbedHelper
    {
        private static void ParseInput(string text)
        {
            if (text.Length > 2048)
                throw new Exception("Text should be less than 2048 - use ExtraModuleBase to split any messages");
        }

        internal static Embed CreateEmbed(string text, bool escape)
        {
            var builder = new EmbedBuilder();
            if (escape)
                text = text.EscapeDiscordChars();
            builder.WithDescription(text)
                .WithColor(ColorConstants.InfoColor);
            return builder.Build();
        }

        internal static Embed CreateEmbed(string title, string text, bool escape = true)
        {
            var builder = new EmbedBuilder();
            if (escape)
            {
                text = text.EscapeDiscordChars();
                title = title.EscapeDiscordChars();
            }
            builder.WithDescription(text)
                .WithTitle(title)
                .WithColor(ColorConstants.InfoColor);
            return builder.Build();
        }

        internal static Embed CreateEmbed(string title, string text, Color color)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text.EscapeDiscordChars())
                .WithTitle(title.EscapeDiscordChars())
                .WithColor(color);
            return builder.Build();
        }
    }
}