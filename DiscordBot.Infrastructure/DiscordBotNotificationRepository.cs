using BoundBot.Components.GetMentionedChannels;
using BoundBot.Components.Members;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting;
using Crosscutting.SellixPayload;
using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Interface;
using DiscordSaga.Components.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using static BoundBot.Components.Members.DiscordServerMembersHandler;

namespace DiscordBot.Infrastructure;

public class DiscordBotNotificationRepository : IDiscordBotNotificationRepository
{
    private readonly ILogger<DiscordBotNotificationRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _connectionHandler;

    public DiscordBotNotificationRepository(ILogger<DiscordBotNotificationRepository> logger, IConfiguration configuration, IDiscordConnectionHandler connectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionHandler = connectionHandler;
    }

    async Task IDiscordBotNotificationRepository.NotificationHandler(LicenseNotificationEvent context)
    {
        try
        {
            var client =
                await _connectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

            var deserializePayload =
                JsonConvert.DeserializeObject<SellixPayloadNormal.Root>(context.Payload ??
                                                                        throw new Exception(
                                                                            "Notification Deserialization Failure"));

            if (deserializePayload != null)
            {
                var socketGuild = client.socketClient.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));
                var restGuild = await client.restClient.GetGuildAsync(ulong.Parse(_configuration["Discord:Guid"]!));

                var guildUser = await client.restClient.GetGuildUserAsync(restGuild.Id, Convert.ToUInt64(deserializePayload.Data.CustomFields.DiscordId));

                bool couldGiveRoleFlag = false;

                try
                {
                    if (guildUser != null && restGuild != null)
                    {
                        if (ulong.TryParse(_configuration["Discord:Role:AIO"], out var aioResult) &&
                            ulong.TryParse(_configuration["Discord:Role:Month"], out var monthResult) &&
                            ulong.TryParse(_configuration["Discord:Role:SoD"], out var sodResult))
                        {
                            var roleId = 0UL;

                            foreach (var spec in context.WhichSpec)
                            {
                                roleId = spec switch
                                {
                                    WhichSpec.AIO => aioResult,
                                    WhichSpec.Placeholder => sodResult,
                                    _ => monthResult
                                };

                                var role = restGuild.GetRole(roleId);

                                if (role != null)
                                {
                                    if (!guildUser.RoleIds.Contains(roleId)) 
                                    {
                                        await guildUser.AddRoleAsync(role);
                                        await guildUser.UpdateAsync();
                                    }
                                }
                                else
                                {
                                    _logger.LogError("Role not found in the guild.");
                                }

                                couldGiveRoleFlag = guildUser.RoleIds.Contains(roleId);
                            }
                        }
                        else
                        {
                            _logger.LogError("Invalid role IDs in configuration.");
                        }
                    }
                    else
                    {
                        _logger.LogError("GuildUser or Guild is null.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while giving a role to a user.");
                }

                ulong[] payedInfoChannelIds =
                {
                    1134147263191584788, // Setup
                    1134147759235141802, // Download
                    1134147503466483812, // Hek
                    1146872448147529798  // Advanced
                };

                var readChannels = await GetMentionedChannels.GetMentionedForumChannelsMethod(socketGuild!, payedInfoChannelIds);

                bool couldSendToUser = false;

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < deserializePayload.Data.Products.Count; i++)
                {
                    TimeSpan remainingTime = context.Time[i].AddSeconds(10) - DateTime.UtcNow;

                    int remainingDays = remainingTime.Days;
                    int remainingHours = remainingTime.Hours;
                    int remainingMinutes = remainingTime.Minutes;
                    int remainingSeconds = remainingTime.Seconds;
                    string formattedRemainingTime = $"{remainingDays}d:{remainingHours}h:{remainingMinutes}m:{remainingSeconds}s";

                    sb.AppendLine($"**Product({i+1}):** `{deserializePayload.Data.Products[i].Title}`");
                    sb.AppendLine($"**Quantity:** `{context.Quantity[i]}`");
                    sb.AppendLine($"**EndDate:** `{context.Time[i]} | ExpiresIn: {formattedRemainingTime}`");
                    sb.AppendLine();
                }

                try
                {
                    var embed = new EmbedBuilder()
                        .WithThumbnailUrl("https://i.imgur.com/dxCVy9r.png")
                        .WithTitle("Purchase Confirmation")

                        .AddField("User", $"{guildUser!.Mention}", true)
                        .AddField("Notified", $"{couldSendToUser}", true)
                        .AddField("Roled", $"{couldGiveRoleFlag}" +
                            (couldGiveRoleFlag ? string.Empty : "\n@ ADMIN"), true)
                        .AddField("OrderId", $"{deserializePayload.Data.Uniqid}", true)

                        .AddField("Product(s)", $"{(sb.Length > 0 ? sb.ToString() : "Fetch Error")}")

                        .AddField("Information Channels",
                            "Please read the following channels:" +
                            "\n" + readChannels)

                        .AddField("Rules",
                            $"\n\nPlease refrain from using {socketGuild!.GetTextChannel(socketGuild.Channels.First(x => x.Name.ToLower().Contains("presale-chat"))!.Id).Mention ?? socketGuild.GetTextChannel(860603152280584226).Mention}" +
                            $"\nfor help or discussions!")
                        .WithColor(Color.DarkOrange)
                        .WithCurrentTimestamp()
                    .Build();

                    await guildUser.SendMessageAsync("", false, embed);
                    couldSendToUser = true;
                }
                catch
                {
                    _logger.LogInformation("Wasn't able to DM: " +
                                           deserializePayload.Data.CustomFields.DiscordUser);
                }

                var privateEmbed = new EmbedBuilder()
                    .WithThumbnailUrl("https://i.imgur.com/dxCVy9r.png")
                    .WithTitle("Purchase Confirmation")
                    
                    .AddField("User", $"{guildUser?.Mention ?? "Not Found"}", true)
                    .AddField("Notified", $"{couldSendToUser}", true)
                    .AddField("Roled", $"{couldGiveRoleFlag}" + (couldGiveRoleFlag ? string.Empty : "\n@ ADMIN"), true)
                    .AddField("OrderId", $"{deserializePayload.Data.Uniqid}", true)

                    .AddField("Product(s)", $"{(sb.Length > 0 ? sb.ToString() : "Fetch Error")}")

                    .AddField("Information Channels",
                        "Please read the following channels:" +
                        "\n" + readChannels)

                    .AddField("Rules",
                        $"\n\nPlease refrain from using {socketGuild!.GetTextChannel(socketGuild.Channels.First(x => x.Name.ToLower().Contains("presale-chat"))!.Id).Mention ?? socketGuild.GetTextChannel(860603152280584226).Mention}" +
                        $"\nfor help or discussions!")
                    .WithColor(Color.DarkOrange)
                    .WithCurrentTimestamp()
                .Build();

                var privateChannel = socketGuild!.GetChannel(socketGuild.Channels.First(x => x.Name.Contains("notify", StringComparison.CurrentCultureIgnoreCase))!.Id) ?? socketGuild.GetChannel(862658521065848872); //NotifyChannel
                var textNotifier = privateChannel as IMessageChannel;
                await textNotifier!.SendMessageAsync("", false, privateEmbed);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(1, ex.Message);
            throw new Exception(ex.Message);
        }
    }
}