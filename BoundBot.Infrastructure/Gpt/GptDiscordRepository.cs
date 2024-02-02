using BoundBot.Application.Gpt.Interface;
using BoundBot.Application.UpdateHwid.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using BoundBot.Infrastructure.UpdateHwid;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BoundBot.Components.GetOptionValue;
using Gpt.Components;
using IdentityServer4.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BoundBot.Infrastructure.Gpt;

public class GptDiscordRepository : IGptDiscordRepository
{
    private readonly ILogger<GptDiscordRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public GptDiscordRepository(ILogger<GptDiscordRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }

    async Task IGptDiscordRepository.UpdateFtModel(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            DiscordModelDtoRestModel restModel = new(command);

            HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/gateway/API/GPT/JwtRefreshAndGenerate", restModel.Model);
            var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
            var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

            GptModel gpt = new()
            {
                MessagesToCollect = Convert.ToInt32(command.GetOptionValues<string>("amount")!)
            };

            HttpResponseMessage resp = await client.PostAsJsonAsync("/gateway/API/GPT/UpdateModel", gpt);
            var responseBody = await resp.Content.ReadAsStringAsync();

            var embedBuilder = new Discord.EmbedBuilder();
            embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";

            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation($"[POST REST] Successfully executed for {command.CommandName}");

                embedBuilder.AddField("Update AI Model",
                    $"\n{responseBody}");
            }
            else
            {
                embedBuilder.AddField("BadRequest", responseBody);
            }

            embedBuilder.WithColor(Color.DarkOrange);
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