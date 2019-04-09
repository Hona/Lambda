using Discord;

namespace LambdaUI
{
    internal static class EmbedHelper
    {
        internal static Embed CreateEmbed(string text)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text)
                .WithColor(ColorConstants.InfoColor);
            return builder.Build();
        }

        internal static Embed CreateEmbed(string title, string text)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text)
                .WithTitle(title)
                .WithColor(ColorConstants.InfoColor);
            return builder.Build();
        }

        internal static Embed CreateEmbed(string title, string text, Color color)
        {
            var builder = new EmbedBuilder();
            builder.WithDescription(text)
                .WithTitle(title)
                .WithColor(color);
            return builder.Build();
        }
    }
}