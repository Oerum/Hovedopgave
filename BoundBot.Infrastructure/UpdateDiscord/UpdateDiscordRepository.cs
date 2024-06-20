using BoundBot.Application.UpdateDiscord.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Connections;

namespace BoundBot.Infrastructure.UpdateDiscord;

public class UpdateDiscordRepository : IUpdateDiscordRepository
{
    private readonly ILogger<UpdateDiscordRepository> _logger;
    private readonly IDiscordConnectionHandler _ConnectionHandler;
    private readonly IConfiguration _configuration;

    public UpdateDiscordRepository(ILogger<UpdateDiscordRepository> logger, IDiscordConnectionHandler discordConnectionHandler, IConfiguration configuration)
    {
        _logger = logger;
        _ConnectionHandler = discordConnectionHandler;
        _configuration = configuration;
    }

    async Task IUpdateDiscordRepository.UpdateDiscord(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await command.DeferAsync(false);

            DiscordModelDtoRestModel restModel = new(command);

            HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/DiscordBot/JwtRefreshAndGenerate", restModel.Model);
            var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
            var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

            HttpResponseMessage resp = await client.PutAsJsonAsync("/API/DiscordBot/Command/UpdateDiscord", restModel.Model);
            var responseBody = await resp.Content.ReadAsStringAsync();

            var embedBuilder = new Discord.EmbedBuilder();
            embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";
            if (resp.IsSuccessStatusCode)
            {

                embedBuilder.AddField("Update Discord",
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