using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LambdaUI.Data;
using LambdaUI.Data.Access.Bot;

namespace LambdaUI.Discord.Modules
{
    internal class TodoModule : ExtraModuleBase
    {
        public TodoDataAccess TodoDataAccess { get; set; }

        [Command("todo")]
        public async Task Embed(string group = "", string value = "")
        {
            if (group == "" && value == "")
            {
                await ReplyNewEmbed((await TodoDataAccess.GetTodoItemsAsync()).Aggregate("",
                    (currentString, nextItem) => currentString +
                                                 $"'{nextItem.Group}' | '{nextItem.Item}' | {nextItem.Completeted}" +
                                                 Environment.NewLine));
            }
            else if (group != "" && value == "")
            {
                await ReplyNewEmbed((await TodoDataAccess.GetTodoItemsAsync(group)).Aggregate("",
                    (currentString, nextItem) => currentString +
                                                 $"'{nextItem.Group}' | '{nextItem.Item}' | {nextItem.Completeted}" +
                                                 Environment.NewLine));
            }
            else
            {
                await TodoDataAccess.CreateTodoItemAsync(group, value);
                await ReplyNewEmbed("Done.");
            }
        }
    }
}