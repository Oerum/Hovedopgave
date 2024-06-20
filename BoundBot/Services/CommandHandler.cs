using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BoundBot.Services
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfiguration _configuration;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, IConfiguration configuration, ILogger<CommandHandler> logger)
        {
            _client = client;
            _commands = commands;
            _configuration = configuration;
            _logger = logger;
        }

        private ServiceProvider? BotsServiceProvider { get; set; } = null;

        public async Task InstallCommandsAsync(ServiceProvider provider)
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += OnCommandExecutedAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: provider);

            BotsServiceProvider = provider;

            foreach (var command in _commands.Commands)
            {
                _logger.LogInformation($"Text Command Initialized: {command.Name}");
            }
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // We have access to the information of the command executed,
            // the context of the command, and the result returned from the
            // execution in this event.

            // We can tell the user what went wrong
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                var embedBuiler = new EmbedBuilder();
                embedBuiler.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";
                embedBuiler.AddField("Whoops", $"`{context.Message.Content}` is not a `slash (/)` command or `!AI` text command!" +
                    "\n\nMake sure you use a proper slash command, and not sending it as a text command." +
                                                   "\n\nAll available commands can be seen when typing the initial '/' or by using `/help`." +
                                               "\n\nSome commands may be hidden to you depending on role & claims.");
                embedBuiler.WithColor(Color.DarkRed);
                embedBuiler.WithCurrentTimestamp();

                // Now, Let's respond with the embed.
                await context.Message.ReplyAsync(embed: embedBuiler.Build());
            }

            

            // ...or even log the result (the method used should fit into
            // your existing log handler)
            var commandName = command.IsSpecified ? command.Value.Name : "A command";
            Console.WriteLine((new LogMessage(LogSeverity.Info,
                "CommandExecution",
                $"{commandName} was executed at {DateTime.UtcNow}.")));
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message

            var message = messageParam as SocketUserMessage;

            if (message == null) return;

            Console.WriteLine($"content: {message.Content} Bot: {message.Author.IsBot} Channel: {message.Channel.Id}");

            if (message.Author.IsBot != true
                && !string.IsNullOrEmpty(message.Content))
            {
                int argPos = 0;

                if (message.Channel.Id.ToString() == _configuration["Discord:IncorrectChannel:Bot"]
                    || message.Channel.Id.ToString() == 913222623150878751.ToString()
                    || message.Channel.Id.ToString() == 1079815280190033950.ToString()
                    || message.Author.Id == 178146383948283904 && message.Content.StartsWith("!"))
                {

                    // Create a WebSocket-based command context based on the message
                    var context = new SocketCommandContext(_client, message);

                    // Execute the command with the command context we just
                    // created, along with the service provider for precondition checks.
                    await _commands.ExecuteAsync(
                        context: context,
                        argPos: argPos,
                        services: BotsServiceProvider);
                }
            }

            // Create a number to track where the prefix ends and the command begins

            //// Determine if the message is a command based on the prefix and make sure no bots trigger commands
            //if (!(message.HasCharPrefix('!', ref argPos) ||
            //      message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            //    message.Author.IsBot)
            //    return;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        }
    }
}

