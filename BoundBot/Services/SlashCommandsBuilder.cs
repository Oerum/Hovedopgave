﻿using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Crosscutting;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoundBot.Services
{
    internal class SlashCommandsBuilder
    {
        private DiscordSocketClient Client { get; set; }


        public SlashCommandsBuilder(DiscordSocketClient client)
        {
            Client = client;
        }

        public async Task Client_Ready()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = Client.GetGuild(860603152280584222);

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
            guildCommand.AddOption("hwid", ApplicationCommandOptionType.String, "hwid from your A# console", true);
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
            guildCommand.AddOption("hwid", ApplicationCommandOptionType.String, "hwid from your A# console", true);
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
            guildCommand.AddOption("product", ApplicationCommandOptionType.Integer, "Choose product", true , choices: new ApplicationCommandOptionChoiceProperties[]
            {
                new() { Name = "1", Value = 1 },
                new() { Name = "2", Value = 2 },
                new() { Name = "3", Value = 3 },
                new() { Name = "4", Value = 4 },
                new() { Name = "5", Value = 5 },
                new() { Name = "6", Value = 6 },
                new() { Name = "7", Value = 7 },
                new() { Name = "AIO", Value = 39 }
            });
            commands.Add(guildCommand.Build());

            guildCommand = new();
            guildCommand.WithName("embedbuilder");
            guildCommand.WithDescription("post an embed");
            guildCommand.AddOption("channel", ApplicationCommandOptionType.Channel, "which channel to post", true);
            guildCommand.AddOption("icon", ApplicationCommandOptionType.Attachment, "use an icon for the embed", false);
            guildCommand.AddOption("title", ApplicationCommandOptionType.String, "title for embed", true);
            guildCommand.AddOption("message", ApplicationCommandOptionType.String, "message for embed", true);
            guildCommand.AddOption("attachment", ApplicationCommandOptionType.Attachment, "attach a file/folder", false);
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
            guildCommand.AddOption("product", ApplicationCommandOptionType.Integer, "Choose product (empty = same product)", false, choices: new ApplicationCommandOptionChoiceProperties[]
            {
                new() { Name = "1", Value = 1 },
                new() { Name = "2", Value = 2 },
                new() { Name = "3", Value = 3 },
                new() { Name = "4", Value = 4 },
                new() { Name = "5", Value = 5 },
                new() { Name = "6", Value = 6 },
                new() { Name = "7", Value = 7 },
                new() { Name = "AIO", Value = 39 }
            });
            commands.Add(guildCommand.Build());

            try
            {
                await Client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());

                /*
                foreach (var ele in commands)
                {
                    await guild.CreateApplicationCommandAsync(ele);
                }
                */

                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.

                // With global commands we don't need the guild.
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
    }
}
