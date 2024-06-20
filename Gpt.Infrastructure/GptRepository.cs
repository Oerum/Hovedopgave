using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using gpt.application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;


namespace Gpt.Infrastructure;

public class GptRepository : IGptRepository
{
    private readonly ILogger<IGptRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _client;
    private readonly IOpenAiFactory _openAiFactory;

    public GptRepository(ILogger<IGptRepository> logger, IConfiguration configuration, IDiscordConnectionHandler client, IOpenAiFactory openAiFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _client = client;
        _openAiFactory = openAiFactory;
    }

    async Task<string> IGptRepository.UpdateFtModel(int amountToCollect)
    {
        try
        {
            var client = await _client.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

            SocketGuild guild;

            var stopwatch = Stopwatch.StartNew();
            var timeout = TimeSpan.FromSeconds(60);

            do
            {
                guild = client.socketClient.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));

                if (stopwatch.Elapsed >= timeout)
                {
                    break;
                }

            } while (guild.Channels is not { Count: > 0 });

            var bypass = new List<ulong>()
            {
                860815074213232660,
                913222623150878751,
                1094738809121423380,
                1079815280190033950,
                861025323700846592,
                861344917426274305,
                861344762917158952,
                860631193661210675,
                879325830021529630
            };

            var priority = new List<ulong>()
            {
                1134149367465529434,
                1134149240017395782
            };

            var rotationChannel = new List<ulong>()
            {
                1183842431653904425,
                1134144016703832124,
                1134143849313357876,
                1134143914123743253,
                1134144223692726303,
                1134144257196834857,
                1134145249741119661,
                1134144836736405534,
                1134145307031126116,
                1134145893961048155,
                1134145530256171028,
                1134144406975430686,
                1134144598667694080,
                1134143972919480440,
                1134143501085462660,
                1134145957366333460,
                1134143785631227904,
                1134145199942160515,
                1134145569401614407,
                1134145668714332160,
                1134144750899953684,
                1134144990096936980,
                1134145769369255969,
                1134144635074265088,
                1134144491821998161,
                1134144054066692226,
                1134145044996165692,
                1134145730207031356,
                1134144911734747177,
                1134143627954769972,
                1134144941933731950,
                1134144169598795797,
                1134145392301310054,
                1134145853225975990,
                1134145853225975990,
                1134143711421411408,
                1134144334988587038,
                1134145127028371608,
                1134145082522603662,
                1134144552228356199,
                1134144790263505049,

            };

            Directory.CreateDirectory("/app/Gpt_Ft_Model/");

            var channels = guild.Channels;

            await using var fileStream = File.CreateText("/app/Gpt_Ft_Model/messages.jsonl");

            int amountCollected = 0;
            int maxAmount = 2048;


            var customMessages = new[]
            {
                new
                {
                    messages = new []
                    {
                        new { role = "user", content = "It is not working" },
                        new { role = "assistant", content = "**How to Download?**\r\n#\"[Core] Download\" \r\n--> Always the latest, even though update notes, don't necessarily shows the spec/class\r\n\r\n**Extract the zip into `Aimsharp/bin/rotations` **\r\n\r\n**DPS/TANK/Disc** require the addon **Hekili** \r\n:white_small_square: More information at #\"[Hekili] Setup Guide\" \r\n\r\n:exclamation:**YOU NEED TO DO (Automatic):** \r\nAimsharp/bin/settings.txt  --> Add `NoTarget=true` \r\n--> Relaunch Aimsharp --> Enable numlock\r\n\r\n**Needs to look like this:**\r\n`ExtraKeys=false\r\nWowBuild=retail\r\nNoTarget=true `\r\n\r\n**Everything will work properly, assuming your ingame wow graphic settings, addons etc, aren't messing with Aimsharp**\r\n\r\n-------------------------------------------\r\n**What do I do, if it isn't working properly?**\r\n1. Enable Debug — So we can see what’s happening.\r\n2. Check General Macro Tab, To Make Sure All Macros Has Been Created - Make sure no                     addons are creating Macros too!\r\n3. Make Sure No Addons Or Overlays Are Hijacking bindings.\r\n4. Disable All Addons Except A#.\r\n5. Close Wow & Aimsharp, Delete Aimsharp Addon From Addons Folder.\r\n6. Restart PC.\r\n-------------------------------------------\r\n\r\n**If You Find Spells To Add To The Included Txt-files, Let Me Know, So I Can Make It Default**\r\n\r\n**Also Remember To Provide Feedback, If You Want Features, Improvements, Changes Etc**" }
                    }
                },
                new
                {
                    messages = new []
                    {
                        new { role = "user", content = "I need help with hekili" },
                        new { role = "assistant", content = ":exclamation: **Set your layout as you wish!**\r\nRecommended: /hekili Go to `Displays->Multiple->Visibility` set  `Default` to `0`\r\n\r\n:exclamation: **Unbind all Hekili bindings** \r\nGo to `toggle` section unbind everything, or make sure you set them on keybinds that do not interfere with Aimsharp! \r\n`If you keep getting pause/unpause - Snapshot - Rebind those toggles & unbind them again!`\r\n\r\n:grey_exclamation: **Toggles**\r\nYou can also go to `Abilites` section, to set whether to use an ability on cooldown toggle, covenant toggle or just whenever.\r\n\r\n:grey_exclamation: **Custom priorities not necessary (Advanced - Not Recommended)**\r\nYou can change priorities in the `priorities tab` like not cast while moving etc, by enabling modifiers & setting whichever you like \r\n\r\n:grey_exclamation: **WeakAura  (not necessary)**\r\nUsed For: `display Mode and Toggles` Instead of the Hekili Bindings: https://wago.io/HekiliToggles \r\n\r\n:exclamation: **Rotation Gets Stuck On Spells (Wowhead Scraper)**\r\nGo to: `Core/TxtFiles/Hekilispells`\r\nAdd `spellid=spellname` to spec file\r\nRestart rotation\r\n\r\nThis will happen automatically, with the wowhead scraper logic:\r\n\r\n> :white_small_square:  Go to dummy, start combat, with cds enabled (Both ST & Cleave).\r\n> \r\n> :white_small_square:  Manually press the spells it suggests if it doesn't automatically.\r\n> \r\n> :white_small_square: The missing `spellids=names` will be included in the txt file.\r\n\r\n> The spellbook has a cap of 50 individual spells (A# limitation) \r\n> \r\n> You can however add a spell to multiple IDs: \r\n> 123=Bloodlust\r\n> 321=Bloodlust\r\n\r\nAnd it'll only count as one individual spell, but the hekili logic is able to pick both up.\r\n\r\n> -- Utility & Defensive spells which aren't used by hekili still have to be manually included.\r\n\r\n\r\n:white_small_square: Will now advance recommendation if current is unusable, due to moving etc (the more buttons you allow it show, the more choices available).\r\n:white_small_square: Will check if you can cast while moving.\r\n:white_small_square: Will correctly count time before using next ability (stationary only).\r\n:white_small_square: Empower To Algorithm Extended To support ^^ (Dracthyr only obviously).\r\n:white_small_square: If no abilities available it'll no longer try to spam an ability, it'll want until x beomes available." }
                    }
                }
            };

            foreach (var messageGroup in customMessages)
            {
                var json = JsonConvert.SerializeObject(messageGroup);
                await fileStream.WriteLineAsync(json);
            }

            amountCollected += customMessages.Length;

            foreach (var item in channels.Where(x => priority.Contains(x.Id)))
            {
                if (client.socketClient.GetChannel(item.Id) is SocketTextChannel channel && !bypass.Contains(item.Id))
                {
                    var messages = await channel.GetMessagesAsync(150).FlattenAsync();
                    var enumerable = messages.ToArray();

                    for (int i = 0; i < enumerable.Count(); i++)
                    {
                        if (amountCollected >= maxAmount)
                        {
                            break;
                        }

                        var message = enumerable[i];
                        var aiRole = "user";

                        if (message.Author.Id == 178146383948283904
                            || message.Author.Id == 736338658126462988)
                        {
                            aiRole = "assistant";
                        }

                        var pairedMessages = new List<object>();

                        if (aiRole == "assistant")
                        {
                            for (int j = i + 1; j < enumerable.Count(); j++)
                            {
                                var nextMessage = enumerable[j];
                                if (nextMessage.Author.Id != message.Author.Id)
                                {
                                    pairedMessages.Add(new { role = "assistant", content = message.Content });
                                    pairedMessages.Add(new { role = "user", content = nextMessage.Content });
                                    i = j;
                                    break;
                                }
                            }
                        }

                        if (pairedMessages.Count > 0)
                        {
                            // Build the outer structure with "messages" key
                            var jsonData = new
                            {
                                messages = pairedMessages
                            };

                            var json = JsonConvert.SerializeObject(jsonData);

                            // Write the JSONL content to the file
                            await fileStream.WriteLineAsync(json);

                            amountCollected++;
                        }
                    }
                }
            }

            if (amountCollected < maxAmount)
            {
                foreach (var item in channels.Where(x => rotationChannel.Contains(x.Id)))
                {
                    if (client.socketClient.GetChannel(item.Id) is SocketTextChannel channel && !bypass.Contains(item.Id))
                    {
                        var messages = await channel.GetMessagesAsync(150).FlattenAsync();
                        var enumerable = messages.ToArray();

                        for (int i = 0; i < enumerable.Count(); i++)
                        {
                            if (amountCollected >= maxAmount)
                            {
                                break;
                            }

                            var message = enumerable[i];
                            var aiRole = "user";

                            if (message.Author.Id == 178146383948283904
                                || message.Author.Id == 736338658126462988)
                            {
                                aiRole = "assistant";
                            }

                            var pairedMessages = new List<object>();

                            if (aiRole == "assistant")
                            {
                                for (int j = i + 1; j < enumerable.Count(); j++)
                                {
                                    var nextMessage = enumerable[j];
                                    if (nextMessage.Author.Id != message.Author.Id)
                                    {
                                        pairedMessages.Add(new { role = "assistant", content = message.Content });
                                        pairedMessages.Add(new { role = "user", content = nextMessage.Content });
                                        i = j;
                                        break;
                                    }
                                }
                            }

                            if (pairedMessages.Count > 0)
                            {
                                // Build the outer structure with "messages" key
                                var jsonData = new
                                {
                                    messages = pairedMessages
                                };

                                var json = JsonConvert.SerializeObject(jsonData);

                                // Write the JSONL content to the file
                                await fileStream.WriteLineAsync(json);

                                amountCollected++;
                            }
                        }
                    }
                }
            }

            await fileStream.DisposeAsync();

            var models = await _openAiFactory.Create().File.AllAsync();

            foreach (var model in models)
            {
                if (model.Id != null) await _openAiFactory.Create().File.DeleteAsync(model.Id);
            }

            var ftFile = File.OpenRead("/app/Gpt_Ft_Model/messages.jsonl");
            var upload = await _openAiFactory.Create().File.UploadFileAsync(ftFile, "fine-tune-custom");

            List<FileResult> allFiles;

            var sw = Stopwatch.StartNew();

            do
            {
                allFiles = await _openAiFactory.Create().File.AllAsync();

                if (stopwatch.Elapsed >= timeout)
                    break;
            }
            while (allFiles[0].Status != "processed");


            var trainingFileId = allFiles[0].Id;
            if (trainingFileId != null)
            {
                var ft = await _openAiFactory.Create().FineTune.Create(trainingFileId).WithModel("gpt-3.5-turbo").ExecuteAsync();
            }

            _logger.LogInformation("Uploaded Model: {Upload}", upload);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id: " + upload.Id);
            sb.AppendLine("Name: " + upload.Name);
            var created = DateTimeOffset.FromUnixTimeSeconds(upload.CreatedAt);
            sb.AppendLine("Created: " + created);
            sb.AppendLine("Total Messages: " + amountCollected + " / 2048");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing Discord messages: {ErrorMessage}", ex.Message);
        }

        throw new Exception("Unhandled Model Upload Error");
    }

    async Task<string> IGptRepository.Gpt(string questions)
    {
        try
        {
            var openAiAPI = _openAiFactory.Create();

            var results = await openAiAPI.Chat
                .Request(new ChatMessage { Role = ChatRole.User, Content = questions })
                .WithModel("ft:gpt-3.5-turbo-0613:personal::7rZSdkKE")
                .WithAllFunctions()
                .WithTemperature(1)
                .AddSystemMessage("You're an assistant created by master Bound, the godlike c# developer, who's made these otherworldly rotations!")
                .AddSystemMessage("Length of your answer must not be larger than 1024 characters in length")
                .ExecuteAsync();

            var messageContent = results.Choices?[0].Message?.Content?.ToString();
            
            if (!string.IsNullOrEmpty(messageContent))
                return messageContent;

            throw new Exception("AI Model Error: Too Large, Invalid or Not Yet Processed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gpt Error {ErrorMessage}", ex.Message);
        }

        throw new Exception("Unhandled AI Error!");
    }
}