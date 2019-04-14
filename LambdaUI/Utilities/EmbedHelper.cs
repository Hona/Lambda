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

        internal static Embed CreateEmbed(string text)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text.EscapeDiscordChars())
                .WithColor(ColorConstants.InfoColor);
            return builder.Build();
        }

        internal static Embed CreateEmbed(string title, string text)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text.EscapeDiscordChars())
                .WithTitle(title.EscapeDiscordChars())
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