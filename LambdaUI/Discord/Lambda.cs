using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LambdaUI.Constants;
using LambdaUI.Data.Access;
using LambdaUI.Data.Access.Bot;
using LambdaUI.Data.Access.Simply;
using LambdaUI.Discord.Updaters;
using LambdaUI.Logging;
using LambdaUI.Services;
using LambdaUI.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace LambdaUI.Discord
{
    public class Lambda
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private ConfigDataAccess _configDataAccess;

        private Timer _intervalFunctionTimer;
        private JustJumpDataAccess _justJumpDataAccess;
        private IServiceProvider _services;
        private SimplyDataUpdater _simplyDataUpdater;
        private SimplyHightowerDataAccess _simplyHightowerDataAccess;

        private SimplyTFServerUpdater _simplyTFServerUpdater;

        private DateTime _startDateTime;
        private TempusActivityUpdater _tempusActivityUpdater;

        private TempusDataAccess _tempusDataAccess;

        private TempusServerUpdater _tempusServerUpdater;
        private TodoDataAccess _todoDataAccess;

        private static int FromMinutes(int minutes) => 1000 * 60 * minutes;

        internal async Task StartAsync()
        {
            _startDateTime = DateTime.Now;

            PrintDisplay();

            InitializeVariables();

            AddClientEvents();

            await Login();

            BuildServiceProvider();

            await InstallCommands();

            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private static void PrintDisplay()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(
                $"                 ----------------===================----------------          {Environment.NewLine}{Environment.NewLine}" +
                $"     ▄█          ▄████████   ▄▄▄▄███▄▄▄▄   ▀█████████▄  ████████▄     ▄████████ {Environment.NewLine}" +
                $"   ███         ███    ███ ▄██▀▀▀███▀▀▀██▄   ███    ███ ███   ▀███   ███    ███ {Environment.NewLine}" +
                $"   ███         ███    ███ ███   ███   ███   ███    ███ ███    ███   ███    ███ {Environment.NewLine}" +
                $"   ███         ███    ███ ███   ███   ███  ▄███▄▄▄██▀  ███    ███   ███    ███ {Environment.NewLine}" +
                $"   ███       ▀███████████ ███   ███   ███ ▀▀███▀▀▀██▄  ███    ███ ▀███████████ {Environment.NewLine}" +
                $"   ███         ███    ███ ███   ███   ███   ███    ██▄ ███    ███   ███    ███ {Environment.NewLine}" +
                $"   ███▌    ▄   ███    ███ ███   ███   ███   ███    ███ ███   ▄███   ███    ███ {Environment.NewLine}" +
                $"   █████▄▄██   ███    █▀   ▀█   ███   █▀  ▄█████████▀  ████████▀    ███    █▀  {Environment.NewLine}" +
                $"   ▀                                                                           {Environment.NewLine}" +
                $"                 ----------------===================----------------          {Environment.NewLine}");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private void InitializeVariables()
        {
            _client = new DiscordSocketClient(
                new DiscordSocketConfig {AlwaysDownloadUsers = true, MessageCacheSize = 50});
            _commands = new CommandService(new CommandServiceConfig {DefaultRunMode = RunMode.Async});

            var connectionStrings = File.ReadAllLines(DiscordConstants.DatabaseInfoPath);
            _tempusDataAccess = new TempusDataAccess();
            _todoDataAccess = new TodoDataAccess(connectionStrings[0]);
            _configDataAccess = new ConfigDataAccess(connectionStrings[0]);
            _justJumpDataAccess = new JustJumpDataAccess(connectionStrings[1]);
            _simplyHightowerDataAccess = new SimplyHightowerDataAccess(connectionStrings[2]);

            _tempusServerUpdater = new TempusServerUpdater(_client, _configDataAccess, _tempusDataAccess);
            _tempusActivityUpdater = new TempusActivityUpdater(_client, _configDataAccess, _tempusDataAccess);
            _simplyTFServerUpdater = new SimplyTFServerUpdater(_client, _configDataAccess);
            _simplyDataUpdater = new SimplyDataUpdater(_client, _configDataAccess,
                new SimplyDataService(_simplyHightowerDataAccess, _justJumpDataAccess));
        }

        private void AddClientEvents()
        {
            _client.Log += Logger.Log;
            _client.MessageReceived += MessageReceived;
            _client.Ready += Ready;
        }


        private async Task Login()
        {
            try
            {
                Logger.LogInfo("Lambda", "Token: " + DiscordConstants.TokenPath);

                var token = File.ReadAllText(DiscordConstants.TokenPath);
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
            // Ignore TF2RJweekly messages for now
            if (messageParam.Channel is SocketGuildChannel socketGuildChannel &&
                socketGuildChannel.Guild.Id == 310494288570089472) return;
            // Create a number to track where the prefix ends and the command begins
            var commandPosition = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(DiscordConstants.CommandPrefix, ref commandPosition) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref commandPosition))) return;

            // Create a Command Context
            var context = new CommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, commandPosition, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync("", embed: EmbedHelper.CreateEmbed(result.ErrorReason, false));
        }


        private async Task Ready()
        {
            if (_client.GetChannel(
                Convert.ToUInt64((await _configDataAccess.GetConfigAsync("logChannel")).First()
                    .Value)) is ITextChannel channel)
                Logger.StartLoggingToChannel(_client, channel);
            Logger.LogInfo("Lambda",
                $"Time elapsed since startup {(DateTime.Now - _startDateTime).TotalMilliseconds}ms");
            await _client.SetGameAsync("!help");

            // Runs once on startup, make sure it runs when connected
            _intervalFunctionTimer = new Timer(IntervalFunctions, null, 0, FromMinutes(5));
        }

        internal async void IntervalFunctions(object state)
        {
            var startDateTime = DateTime.Now;
            var tasks = new List<Task>
            {
                _tempusDataAccess.UpdateMapListAsync(),
                _tempusServerUpdater.UpdateServers(),
                _tempusServerUpdater.UpdateOverviewsAsync(),
                _tempusActivityUpdater.UpdateActivity(),
                _simplyTFServerUpdater.UpdateServers(),
                _simplyDataUpdater.UpdateData()
            };
            await Task.WhenAll(tasks);

            Logger.LogInfo("Lambda", $"Interval functions took {(DateTime.Now - startDateTime).TotalMilliseconds}ms");
        }

        private void BuildServiceProvider()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_tempusDataAccess)
                .AddSingleton(_todoDataAccess)
                .AddSingleton(_configDataAccess)
                .AddSingleton(_justJumpDataAccess)
                .AddSingleton(this)
                .BuildServiceProvider();
        }

        private async Task InstallCommands()
        {
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}