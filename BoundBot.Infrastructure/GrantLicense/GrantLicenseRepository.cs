using BoundBot.Application.GrantLicense.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BoundBot.Components.GetOptionValue;

namespace BoundBot.Infrastructure.GrantLicense;

public class GrantLicenseRepository : IGrantLicenseRepository
{
    private readonly ILogger<GrantLicenseRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public GrantLicenseRepository(ILogger<GrantLicenseRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }


    async Task IGrantLicenseRepository.GrantLicense(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            DiscordModelDtoRestModel restModel = new(command);
            var embedBuilder = new Discord.EmbedBuilder
            {
                ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png"
            };

            if (restModel.Model.Roles!.Contains(_configuration["Discord:Role:Admin"]!) || restModel.Model.Roles.Contains(_configuration["Discord:Role:Staff"]!))
            {
                if (command.Data.Options.Count > 0)
                {
                    var minutesToExtend = command.GetOptionValues<string>("minutestogrant");
                    var discordId = command.GetOptionValues<IUser>("user");
                    var discordUsername = command.GetOptionValues<IUser>("user");
                    var product = int.TryParse(command.GetOptionValues<string>("product"), out var parsedProduct) ? parsedProduct : 0;
                    var userHwid = command.GetOptionValues<string>("hwid");

                    var postModel = new GrantLicenseDto()
                    {
                        MinutesToExtend = minutesToExtend,
                        DiscordId = discordId!.Id.ToString(),
                        DiscordUsername = discordUsername!.Username,
                        Product = (WhichSpec)parsedProduct,
                        Hwid = userHwid
                    };

                    HttpResponseMessage jwtResponseMessage =
                        await client.PostAsJsonAsync($"/gateway/API/BC/Admin/JwtRefreshAndGenerate",
                            restModel.Model);
                    var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                    var jwtResponseBodyDeserialization =
                        JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                    HttpResponseMessage resp = await client.PostAsJsonAsync($"/gateway/API/BC/Admin/GrantLicense", postModel);
                    var responseBody = await resp.Content.ReadAsStringAsync();

                    if (resp.IsSuccessStatusCode)
                    {
                        embedBuilder.AddField("License Granted",
                            $"\n\n{responseBody}");

                        embedBuilder.WithColor(Color.DarkOrange);
                    }
                    else
                    {
                        embedBuilder.AddField("BadRequest", responseBody);
                    }
                }
            }
            else
            {
                embedBuilder.AddField("Denied", "You do not have access to staff commands!");
            }

            embedBuilder.WithCurrentTimestamp();

            try
            {
                await command.RespondAsync(embed: embedBuilder.Build());
            }
            catch (Exception ex)
            {
                try
                {
                    DiscordSocketClient discordClient =
                        _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                    var clientUser = await discordClient.GetUserAsync(Convert.ToUInt64(restModel.Model.DiscordId));

                    var privateChannel = await discordClient.GetChannelAsync(Convert.ToUInt64(restModel.Model.Channel)); //Exec channel
                    var textNotifier = privateChannel as IMessageChannel;
                    await textNotifier!.SendMessageAsync($"{clientUser.Mention}\n*Discord API 3S Respond TD Expired\n[Reverting To Channel Message]*", false, embedBuilder.Build());
                }
                catch
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}