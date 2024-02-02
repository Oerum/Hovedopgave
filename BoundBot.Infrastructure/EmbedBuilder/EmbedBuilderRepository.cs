using BoundBot.Application.EmbedBuilder.Interface;
using Discord;
using Discord.WebSocket;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BoundBot.Components.GetOptionValue;
using Exception = System.Exception;

namespace BoundBot.Infrastructure.EmbedBuilder;

public class EmbedBuilderRepository : IEmbedBuilderRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmbedBuilderRepository> _logger;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public EmbedBuilderRepository(IConfiguration configuration, ILogger<EmbedBuilderRepository> logger, IDiscordConnectionHandler discordConnectionHandler)
    {
        _configuration = configuration;
        _logger = logger;
        _discordConnectionHandler = discordConnectionHandler;
    }

    async Task IEmbedBuilderRepository.EmbedBuilder(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            DiscordModelDtoRestModel restModel = new(command);
            var embedBuilder = new Discord.EmbedBuilder();
            embedBuilder.WithColor(Color.DarkOrange);

            embedBuilder.WithCurrentTimestamp();


            bool hasAccess = restModel.Model.Roles!.Contains(_configuration["Discord:Role:Admin"]!) ||
                             restModel.Model.Roles.Contains(_configuration["Discord:Role:Staff"]!);

            ulong channelId = command.Channel.Id;
            IAttachment? attachment = null;

            if (command.Data.Options.Count > 0)
            {
                switch (hasAccess)
                {
                    case true:
                        var channelOption = command.GetOptionValues<IGuildChannel>("channel");
                        channelId = channelOption?.Id ?? command.Channel.Id;
                        var icon = command.GetOptionValues<IAttachment>("icon")!;
                        var title = command.GetOptionValues<string>("title")!;
                        var message = command.GetOptionValues<string>("message")!;
                        attachment = command.GetOptionValues<IAttachment>("attachment")!;

                        if (!string.IsNullOrEmpty(icon.Url))
                        {
                            //var useIcon = await client.GetAsync(icon.Url);
                            embedBuilder.ThumbnailUrl = icon.Url;
                        }

                        embedBuilder.AddField(title,
                            $"\n\n{message}");
                        break;

                    case false:
                        embedBuilder.AddField("Denied", "You do not have access to staff commands!");
                        try
                        {
                            await command.RespondAsync(embed: embedBuilder.Build());
                        }
                        catch (Exception)
                        {
                            DiscordSocketClient discordClient =
                                _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                            var privateChannel = await discordClient.GetChannelAsync(Convert.ToUInt64(channelId)); //Exec channel
                            var textNotifier = privateChannel as IMessageChannel;
                            await textNotifier!.SendMessageAsync(embed: embedBuilder.Build());
                        }
                        return;
                }
            }
            else
            {
                embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";
                embedBuilder.AddField("Error", "Option Count Below 1");
            }

            bool respond = channelId == command.Channel.Id;
            bool attach = attachment != null;

            if (respond)
            {
                if (attach)
                {
                    try
                    {
                        var file = await client.GetAsync(attachment?.Url);
                        // Send the file as an attachment using RespondWithFileAsync
                        await command.RespondWithFileAsync(await file.Content.ReadAsStreamAsync(), Path.GetFileName(attachment?.Url));
                    }
                    catch (Exception)
                    {
                        DiscordSocketClient discordClient =
                            _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                        var privateChannel = await discordClient.GetChannelAsync(Convert.ToUInt64(channelId)); //Exec channel
                        var textNotifier = privateChannel as IMessageChannel;

                        var file = await client.GetAsync(attachment?.Url);

                        var sendAttachment = new FileAttachment(await file.Content.ReadAsStreamAsync(), Path.GetFileName(attachment?.Url));
                        await textNotifier!.SendMessageAsync(embed: embedBuilder.Build());
                        await textNotifier!.SendFileAsync(attachment: sendAttachment);
                    }
                }
                else
                {
                    try
                    {
                        await command.RespondAsync(embed: embedBuilder.Build());
                    }
                    catch (Exception)
                    {
                        DiscordSocketClient discordClient =
                            _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                        var privateChannel = await discordClient.GetChannelAsync(Convert.ToUInt64(channelId)); //Exec channel
                        var textNotifier = privateChannel as IMessageChannel;
                        await textNotifier!.SendMessageAsync(embed: embedBuilder.Build());
                    }
                }
            }
            else
            {
                DiscordSocketClient discordClient =
                    _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                var privateChannel = await discordClient.GetChannelAsync(Convert.ToUInt64(channelId)); //Exec channel
                var textNotifier = privateChannel as IMessageChannel;

                if (attach)
                {
                    var file = await client.GetAsync(attachment?.Url);

                    var sendAttachment = new FileAttachment(await file.Content.ReadAsStreamAsync(), Path.GetFileName(attachment?.Url));
                    await textNotifier!.SendFileAsync(attachment: sendAttachment, embed: embedBuilder.Build());
                }
                else
                {
                    await textNotifier!.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
