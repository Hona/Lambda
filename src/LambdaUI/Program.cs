using LambdaUI.Discord;

namespace LambdaUI
{
    public static class Program
    {
        // Start the bot
        public static void Main()
        {
            new Lambda().StartAsync().GetAwaiter().GetResult();
        }
    }
}