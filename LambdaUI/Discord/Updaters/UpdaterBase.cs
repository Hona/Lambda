using System.Threading.Tasks;
using Discord;

namespace LambdaUI.Discord.Updaters
{
    public class UpdaterBase
    {
        protected async Task DeleteAllMessages(ITextChannel channel)
        {
            var messages = await channel.GetMessagesAsync().FlattenAsync();
            await channel.DeleteMessagesAsync(messages);
        }
    }
}