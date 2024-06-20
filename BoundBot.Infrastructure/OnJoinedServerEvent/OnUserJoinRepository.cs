using BoundBot.Application.OnJoinedServerEvent.JoinMessage.Interface;
using BoundBot.Components.GetMentionedChannels;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BoundBot.Infrastructure.OnJoinedServerEvent;

public class OnUserJoinRepository : IOnUserJoinRepository
{
    private readonly ILogger<OnUserJoinRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public OnUserJoinRepository(ILogger<OnUserJoinRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }

    async Task IOnUserJoinRepository.UserJoined(SocketGuildUser user)
    {
        try
        {
            _logger.LogInformation("User Join Event Fired: " + user.Username);

            var guild = user.Guild;
            var channel = guild.GetTextChannel(guild.Channels.First(x => x.Name.ToLower().Contains("welcome"))!.Id) ?? guild.GetTextChannel(860631193661210675);

            ulong[] infoChannelIds =
            {
                1146535538971656264, // AboutUs
                912356133937758300,  // FAQ
                1095673161862893608  // Purchase
            };

            ulong[] payedInfoChannelIds =
            {
                1134147263191584788, // Setup
                1134147759235141802, // Download
                1134147503466483812, // Hek
                1146872448147529798  // Advanced
            };

            var mentionedChannels = await GetMentionedChannels.GetMentionedForumChannelsMethod(guild, infoChannelIds);
            var payedMentionedChannels = await GetMentionedChannels.GetMentionedForumChannelsMethod(guild, payedInfoChannelIds);

            var welcomeEmbed = new Discord.EmbedBuilder()
                        .AddField($"Welcome 🎉",
                            $"{user.Mention} before you ask questions, please make sure to check our" +
                            "\ninformation channels and FAQs to find answers to common queries." +
                            "\nThis will help you get started and find what you need more efficiently. " +
                            "\nIf you still have questions after that, feel free to ask! <:peepolove:1002285157132271746>")
                        
                        .AddField("Information Channels",
                            mentionedChannels)

                        .AddField("Member Information Channels (Unlock upon purchase)",
                            payedMentionedChannels)

                        .WithColor(Color.DarkOrange)
                        .WithCurrentTimestamp()
                        .Build();
                
            if (channel != null)
            {
                // Send the welcome message
                await channel.SendMessageAsync("", false, welcomeEmbed);
            }

            try
            {
                // Send a direct message to the user
                await user.SendMessageAsync("", false, welcomeEmbed);
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, "OnJoin Event DM Error!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(1, ex.Message);
        }
    }
}