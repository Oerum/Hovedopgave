using BoundBot.Application.ExtendLicenses.Interface;
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

namespace BoundBot.Infrastructure.ExtendLicenses;

public class ExtendLicensesRepository : IExtendLicensesRepository
{
    private readonly ILogger<ExtendLicensesRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public ExtendLicensesRepository(ILogger<ExtendLicensesRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }


    async Task IExtendLicensesRepository.Extend(SocketSlashCommand command, HttpClient client)
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
                    var minutestoExtend = command.GetOptionValues<string>("minutestoextend");

                    IUser? discordid = null; 

                    if (command.Data.Options.ElementAtOrDefault(1)!.Name != null)
                    {
                        discordid = command.GetOptionValues<IUser>("user");
                    }

                    var postModel = new ExtendLicenseDto
                    {
                        MinutesToExtend = minutestoExtend,
                        DiscordId = discordid?.Id.ToString(),
                    };

                    HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/gateway/API/BC/Admin/JwtRefreshAndGenerate", restModel.Model);
                    var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                    var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                    HttpResponseMessage resp = await client.PostAsJsonAsync($"/gateway/API/BC/Admin/ExtendLicenses", postModel);

                    var responseBody = await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        embedBuilder.AddField("ExtendLicense(s)",
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