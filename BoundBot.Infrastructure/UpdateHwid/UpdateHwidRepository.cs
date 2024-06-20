using BoundBot.Application.UpdateHwid.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BoundBot.Components.GetOptionValue;
using Microsoft.AspNetCore.Connections;

namespace BoundBot.Infrastructure.UpdateHwid;

public class UpdateHwidRepository : IUpdateHwidRepository
{
    private readonly ILogger<UpdateHwidRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _ConnectionHandler;

    public UpdateHwidRepository(ILogger<UpdateHwidRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _ConnectionHandler = discordConnectionHandler;
    }


    async Task IUpdateHwidRepository.UpdateHwid(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await command.DeferAsync(false);

            DiscordModelDtoRestModel restModel = new(command)
            {
                Model =
                {
                    Hwid = command.GetOptionValues<string>("hwid")!
                }
            };

            HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/DiscordBot/JwtRefreshAndGenerate", restModel.Model);
            var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
            var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

            HttpResponseMessage resp = await client.PutAsJsonAsync("/API/DiscordBot/Command/UpdateHwid", restModel.Model);
            var responseBody = await resp.Content.ReadAsStringAsync();

            var embedBuilder = new Discord.EmbedBuilder();
            embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";

            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation($"[POST REST] Successfully executed for {command.CommandName}");

                embedBuilder.AddField("Hwid Reset",
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
                await command.ModifyOriginalResponseAsync(x =>
                {
                    x.Content = null; 
                    x.Embed = embedBuilder.Build(); 
                });
            }
            catch (Exception ex)
            {
                try
                {
                    var discordClient =
                           await _ConnectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

                    var clientUser = await discordClient.socketClient.GetUserAsync(Convert.ToUInt64(restModel.Model.DiscordId));

                    var privateChannel = await discordClient.socketClient.GetChannelAsync(Convert.ToUInt64(restModel.Model.Channel)); //Exec channel
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