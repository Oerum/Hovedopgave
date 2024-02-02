using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;
using Discord.Rest;
using Microsoft.Extensions.Logging;

namespace BoundBot.Connection.DiscordConnectionHandler
{
    namespace DiscordClientLibrary
    {
        public interface IDiscordConnectionHandler
        {
            public DiscordSocketClient GetDiscordSocketClient(string token);
            public Task<(DiscordSocketClient socketClient, DiscordRestClient restClient)> GetDiscordSocketRestClient(string token);
            public CommandService GetCommandService();
        }

        public class DiscordConnectionHandler : IDiscordConnectionHandler
        {
            private readonly ILogger<DiscordConnectionHandler> _logger;

            public DiscordConnectionHandler(ILogger<DiscordConnectionHandler> logger)
            {
                _logger = logger;
            }

            DiscordSocketClient IDiscordConnectionHandler.GetDiscordSocketClient(string token)
            {
                DiscordSocketClient client = new(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    GatewayIntents = GatewayIntents.All,
                });

                if (client.ConnectionState != ConnectionState.Connected)
                {

                    client.SetGameAsync("You!", null, ActivityType.Watching).Wait();
                    client.LoginAsync(TokenType.Bot, token).Wait();
                    client.StartAsync().Wait();

                    var stopwatch = Stopwatch.StartNew();
                    var timeout = TimeSpan.FromSeconds(60);

                    while (client.ConnectionState != ConnectionState.Connected)
                    {
                        if (stopwatch.Elapsed >= timeout)
                        {
                            _logger.LogInformation(1, "Failed to connect to Discord within the timeout period.");
                            break;
                        }

                        Task.Delay(100).Wait();
                    }
                }

                return client;
            }

            public async Task<(DiscordSocketClient socketClient, DiscordRestClient restClient)> GetDiscordSocketRestClient(string token)
            {
                try
                {
                    DiscordSocketClient socketClient = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true,
                        GatewayIntents = GatewayIntents.All,
                    });

                    DiscordRestClient restClient = new DiscordRestClient(new DiscordRestConfig
                    {
                        LogLevel = LogSeverity.Info, // Set the desired log level
                        DefaultRetryMode = RetryMode.AlwaysRetry // Set your retry mode preference
                    });

                    if (socketClient.ConnectionState != ConnectionState.Connected)
                    {

                        await socketClient.SetGameAsync("You!", null, ActivityType.Watching);
                        await socketClient.LoginAsync(TokenType.Bot, token);
                        await socketClient.StartAsync();

                        var stopwatch = Stopwatch.StartNew();
                        var timeout = TimeSpan.FromSeconds(60);

                        while (socketClient.ConnectionState != ConnectionState.Connected)
                        {
                            if (stopwatch.Elapsed >= timeout)
                            {
                                _logger.LogInformation(1,
                                    "Failed to connect [Socket] to Discord within the timeout period.");
                                break;
                            }

                            Task.Delay(100).Wait();
                        }
                    }

                    if (restClient.LoginState != LoginState.LoggedIn)
                    {
                        await restClient.LoginAsync(TokenType.Bot, token);

                        var stopwatch = Stopwatch.StartNew();
                        var timeout = TimeSpan.FromSeconds(60);

                        while (restClient.LoginState != LoginState.LoggedIn)
                        {
                            if (stopwatch.Elapsed >= timeout)
                            {
                                _logger.LogInformation(1,
                                    "Failed to connect [REST] to Discord within the timeout period.");
                                break;
                            }

                            await Task.Delay(100);
                        }
                    }

                    return (socketClient, restClient);
                }
                catch (Exception ex)
                {
                    _logger.LogError(1, ex, "Discord Connection Handler Error!");
                }

                return (null, null)!;
            }

            CommandService IDiscordConnectionHandler.GetCommandService()
            {
                CommandService command = new CommandService();

                command ??= new CommandService();

                return command;
            }
        }
    }
}
