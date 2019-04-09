using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace LambdaUI
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private TempusDataAccess _tempusDataAccess;
        private TodoDataAccess _todoDataAccess;
        private ConfigDataAccess _configDataAccess;

        private Timer _intervalFunctionTimer;

        private TempusServerUpdater _tempusServerUpdater;
        private TempusActivityUpdater _tempusActivityUpdater;

        private static int FromMinutes(int minutes) => 1000 * 60 * minutes;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            InitializeVariables();

            AddClientEvents();

            await Login();

            BuildServiceProvider();

            await InstallCommands();

            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private void InitializeVariables()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true });
            _commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });

            

            var connectionStrings = File.ReadAllLines(Constants.DatabaseInfoPath);
            _tempusDataAccess = new TempusDataAccess();
            _todoDataAccess = new TodoDataAccess(connectionStrings[0]);
            _configDataAccess = new ConfigDataAccess(connectionStrings[0]);

            _tempusServerUpdater = new TempusServerUpdater(_client, _configDataAccess, _tempusDataAccess );
            _tempusActivityUpdater = new TempusActivityUpdater(_client, _configDataAccess, _tempusDataAccess);
            //_rankUpdater = new RankUpdater(_client, _simplyDataAccess);
            //_statusUpdater = new StatusUpdater(_client);

            _intervalFunctionTimer = new Timer(IntervalFunctions, null, 0, FromMinutes(5));
        }
        private void AddClientEvents()
        {
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.Ready += Ready;
        }


        private async Task Login()
        {
            try
            {
                Console.WriteLine(Constants.TokenPath);

                var token = File.ReadAllText(Constants.TokenPath);
                await _client.LoginAsync(TokenType.Bot, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private async Task MessageReceived(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            if (!(messageParam is SocketUserMessage message)) return;

            // Create a number to track where the prefix ends and the command begins
            var commandPosition = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(Constants.CommandPrefix, ref commandPosition) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref commandPosition))) return;

            // Create a Command Context
            var context = new CommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, commandPosition, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync("", embed: EmbedHelper.CreateEmbed(result.ErrorReason));
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private async Task Ready()
        {
           // IntervalFunctions(null);
            await _client.SetGameAsync("!help");
        }
        internal async void IntervalFunctions(object state)
        {
            await _tempusDataAccess.UpdateMapList();
            //await _tempusServerUpdater.UpdateServers();
            await _tempusActivityUpdater.UpdateActivity();

        }
        private void BuildServiceProvider()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_tempusDataAccess)
                .AddSingleton(_todoDataAccess)
                .AddSingleton(_configDataAccess)
                .AddSingleton(this)
                .BuildServiceProvider();

        }
        public async Task InstallCommands()
        {
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }

}
