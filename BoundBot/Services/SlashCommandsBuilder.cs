using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using Crosscutting;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BoundBot.Services
{
    internal class SlashCommandsBuilder
    {
        private DiscordSocketClient Client { get; set; }
        private IConfiguration Configuration { get; set; }
        private ILogger<SlashCommandsBuilder> Logger { get; set; }


        public SlashCommandsBuilder(DiscordSocketClient client, IConfiguration configuration, ILogger<SlashCommandsBuilder> logger)
        {
            Client = client;
            Configuration = configuration;
            Logger = logger;
        }

        public async Task InstallSlashCommandsAsync()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = Client.GetGuild(Convert.ToUInt64(Configuration["Discord:Guid"]));

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.

            List<ApplicationCommandProperties> commands = new();
            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            // Descriptions can have a max length of 100.
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("help");
            guildCommand.WithDescription("Get a list of commands");
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("purchase");
            guildCommand.WithDescription("Get shop link & required information");
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("updatediscord");
            guildCommand.WithDescription("Update your discord name & id for active license(s) & get according role");
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("hwid");
            guildCommand.WithDescription("Update your hwid for active license(s)");
            guildCommand.AddOption("hwid", ApplicationCommandOptionType.String, "hwid from your A console", true);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("checkme");
            guildCommand.WithDescription("Check your active license(s)");
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("coupon");
            guildCommand.WithDescription("Get a booster coupon");
            commands.Add(guildCommand.Build());

            //Staff
            guildCommand = new();
            guildCommand.WithName("checkdb");
            guildCommand.WithDescription("Check active license(s) for user");
            guildCommand.AddOption("user", ApplicationCommandOptionType.User, "User", true);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("stafflicense");
            guildCommand.WithDescription("Grant yourself a staff license");
            guildCommand.AddOption("hwid", ApplicationCommandOptionType.String, "hwid from your A console", true);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("extendlicense");
            guildCommand.WithDescription("extend license(s)");
            guildCommand.AddOption("minutestoextend", ApplicationCommandOptionType.Integer, "Minutes to extend the license(s)", true);
            guildCommand.AddOption("user", ApplicationCommandOptionType.User, "Extend specific user - empty for all", false);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("dbbackup");
            guildCommand.WithDescription("dump the database");
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("grantlicense");
            guildCommand.WithDescription("Grant a license");
            guildCommand.AddOption("minutestogrant", ApplicationCommandOptionType.Integer, "Minutes to grant the license(s)", true);
            guildCommand.AddOption("user", ApplicationCommandOptionType.User, "User", true);
            guildCommand.AddOption("hwid", ApplicationCommandOptionType.String, "The hwid for the user", true);
            guildCommand.AddOption("product", ApplicationCommandOptionType.Integer, "Choose product", true , choices:
            [
                new() { Name = "Holy Paladin", Value = 1 },
                new() { Name = "Restoration Druid", Value = 2 },
                new() { Name = "Restoration Shaman", Value = 3 },
                new() { Name = "Holy Priest", Value = 4 },
                new() { Name = "Discipline Priest", Value = 5 },
                new() { Name = "Preservation Evoker", Value = 6 },
                new() { Name = "Mistweaver Monk", Value = 7 },
                new() { Name = "AIO", Value = 39 },
                new() { Name = "SoD", Value = 40 }
            ]);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("aimodelupdate");
            guildCommand.WithDescription("Update the AI Model");
            guildCommand.AddOption("amount", ApplicationCommandOptionType.Integer, "How many messages per channel to collect?", isRequired: true);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("alterlicense");
            guildCommand.WithDescription("alter a license");
            guildCommand.AddOption("orderid", ApplicationCommandOptionType.String, "OrderId for the license", true);
            guildCommand.AddOption("user", ApplicationCommandOptionType.User, "Update user for license", true);
            guildCommand.AddOption("product", ApplicationCommandOptionType.Integer, "Choose product (empty = same product)", false, choices:
            [
                new() { Name = "Holy Paladin", Value = 1 },
                new() { Name = "Restoration Druid", Value = 2 },
                new() { Name = "Restoration Shaman", Value = 3 },
                new() { Name = "Holy Priest", Value = 4 },
                new() { Name = "Discipline Priest", Value = 5 },
                new() { Name = "Preservation Evoker", Value = 6 },
                new() { Name = "Mistweaver Monk", Value = 7 },
                new() { Name = "AIO", Value = 39 },
                new() { Name = "SoD", Value = 40 }
            ]);
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("reply");
            guildCommand.WithDescription("reply to a user");
            guildCommand.AddOption("response", ApplicationCommandOptionType.String, "response", isRequired: true);
            commands.Add(guildCommand.Build());

            try
            {
                //Do not use guild commands, client commands are global and can be used in any server!

                //var existingCommands = await guild.GetApplicationCommandsAsync();
                //foreach (var command in commands)
                //{
                //    if (existingCommands.Any(x => x.Name == command.Name.ToString()))
                //    {
                //        continue;
                //    }

                //    Logger.LogInformation($"Registering command: {command.Name}");
                //    await guild.CreateApplicationCommandAsync(command);
                //}

                await Client.BulkOverwriteGlobalApplicationCommandsAsync([.. commands]);

                Logger.LogInformation("Slash commands registered");
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Logger.LogError($"Failed to register slash commands: {exception.Message} {json}");
            }
        }
    }
}
