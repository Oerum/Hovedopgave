using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace BoundBot.Infrastructure.CheckLicenses;

public class CheckLicenseRepository : ICheckLicenseRepository
{
    private readonly ILogger<CheckLicenseRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _connectionHandler;
    public CheckLicenseRepository(ILogger<CheckLicenseRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionHandler = discordConnectionHandler;
    }

    async Task ICheckLicenseRepository.CheckLicense(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await command.DeferAsync(false);

            DiscordModelDtoRestModel restModel = new(command);

            HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/DiscordBot/JwtRefreshAndGenerate", restModel.Model);
            var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
            var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

            HttpResponseMessage resp = await client.GetAsync($"/API/DiscordBot/Query/CheckMe/{command.User.Username + " % 23" + command.User.Discriminator}/{command.User.Id}");

            var responseBody = await resp.Content.ReadAsStringAsync();

            var embedBuilder = new Discord.EmbedBuilder();
            embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";
            if (resp.IsSuccessStatusCode)
            {
                var authModels = JsonConvert.DeserializeObject<List<AuthModelDTO>>(responseBody);
                StringBuilder builder = new StringBuilder();

                foreach (var item in authModels!)
                {
                    TimeSpan remainingTime = item.EndDate - DateTime.UtcNow;

                    int remainingDays = remainingTime.Days;
                    int remainingHours = remainingTime.Hours;
                    int remainingMinutes = remainingTime.Minutes;
                    int remainingSeconds = remainingTime.Seconds;
                    string formattedRemainingTime = $"{remainingDays}d:{remainingHours}h:{remainingMinutes}m:{remainingSeconds}s";


                    builder.AppendLine($"**DiscordUsername**: {item.DiscordUsername}");
                    builder.AppendLine($"**DiscordId**: {item.DiscordId}");
                    builder.AppendLine($"**Name**: {item.ProductName}");
                    builder.AppendLine($"**Hwid**: {item.HWID}");
                    builder.AppendLine($"**EndDate:** {item.EndDate} | ExpiresIn: {formattedRemainingTime}");

                    builder.AppendLine("\n");
                }

                embedBuilder.AddField("CheckMe",
                    $"\n{builder}");

                embedBuilder.WithColor(Color.DarkOrange);
            }
            else
            {
                embedBuilder.AddField("BadRequest", responseBody);
            }

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
                           await _connectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

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