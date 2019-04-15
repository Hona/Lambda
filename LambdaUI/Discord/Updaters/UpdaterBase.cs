using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LambdaUI.Data;

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
