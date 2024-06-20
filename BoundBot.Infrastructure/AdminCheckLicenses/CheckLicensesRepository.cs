using BoundBot.Application.AdminCheckLicenses.Interface;
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
using System.Text;
using BoundBot.Components.GetOptionValue;

namespace BoundBot.Infrastructure.AdminCheckLicenses;

public class AdminCheckLicensesRepository : IAdminCheckLicensesRepository
{
    private readonly ILogger<AdminCheckLicensesRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public AdminCheckLicensesRepository(ILogger<AdminCheckLicensesRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }


    async Task IAdminCheckLicensesRepository.CheckLicenses(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await command.DeferAsync(false);

            DiscordModelDtoRestModel restModel = new(command);

            var embedBuilder = new Discord.EmbedBuilder
            {
                ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png"
            };

            if (restModel.Model.Roles!.Contains(_configuration["Discord:Role:Admin"]!) || restModel.Model.Roles.Contains(_configuration["Discord:Role:Staff"]!))
            {

                var options = command.GetOptionValues<IUser>("user");

                HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/DiscordBot/JwtRefreshAndGenerate", restModel.Model);
                var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                HttpResponseMessage resp = await client.GetAsync($"/API/DiscordBot/Query/CheckDB/{options!.Username + "%23" + options.Discriminator}/{options.Id}");
                var responseBody = await resp.Content.ReadAsStringAsync();


                if (resp.IsSuccessStatusCode)
                {
                    var authModels = JsonConvert.DeserializeObject<List<AuthModelDTO>>(responseBody);

                    StringBuilder builder = new StringBuilder();

                    foreach (var item in authModels!)
                    {
                        builder.AppendLine($"**DiscordUsername**: {item.DiscordUsername}");
                        builder.AppendLine($"**DiscordId**: {item.DiscordId}");
                        builder.AppendLine($"**Firstname**: {item.Firstname}");
                        builder.AppendLine($"**Lastname**: {item.Lastname}");
                        builder.AppendLine($"**Email**: {item.Email}");
                        builder.AppendLine($"**Name**: {item.ProductName}");
                        builder.AppendLine($"**Hwid**: {item.HWID}");
                        builder.AppendLine($"**EndDate**: {item.EndDate}");
                        builder.AppendLine("\n");
                    }

                    embedBuilder.AddField("CheckDB",
                        $"\n{builder}");

                    embedBuilder.WithColor(Color.DarkOrange);
                }
                else
                {
                    embedBuilder.AddField("BadRequest", responseBody);
                }
            }
            else
            {
                embedBuilder.AddField("Denied", "You do not have access to staff commands!");
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
                        await _discordConnectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

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