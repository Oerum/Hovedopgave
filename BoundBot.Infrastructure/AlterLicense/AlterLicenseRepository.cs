using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Application.AlterLicense.Interface;
using BoundBot.Components.GetOptionValue;
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

namespace BoundBot.Infrastructure.AlterLicense
{
    public class AlterLicenseRepository : IAlterLicenseRepository
    {
        private readonly ILogger<AlterLicenseRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDiscordConnectionHandler _connectionHandler;

        public AlterLicenseRepository(ILogger<AlterLicenseRepository> logger, IConfiguration configuration, IDiscordConnectionHandler connectionHandler)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionHandler = connectionHandler;
        }

        async Task IAlterLicenseRepository.AlterLicense(SocketSlashCommand command, HttpClient client)
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
                        var orderId = command.GetOptionValues<string>("orderid");
                        var discordId = command.GetOptionValues<IUser>("user");
                        var product = int.TryParse(command.GetOptionValues<string>("product"), out var parsedProduct) ? parsedProduct : 0;

                        var postModel = new AlterLicenseDTO()
                        {
                            OrderId = orderId,
                            DiscordId = discordId!.Id.ToString(),
                            DiscordName = discordId.Username.ToString(),
                            Product = (WhichSpec)parsedProduct,
                        };

                        HttpResponseMessage jwtResponseMessage =
                            await client.PostAsJsonAsync($"/gateway/API/BC/Admin/JwtRefreshAndGenerate",
                                restModel.Model);
                        var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                        var jwtResponseBodyDeserialization =
                            JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                        HttpResponseMessage resp = await client.PutAsJsonAsync($"/gateway/API/BC/Admin/AlterLicense", postModel);
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
                            _connectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

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
}
