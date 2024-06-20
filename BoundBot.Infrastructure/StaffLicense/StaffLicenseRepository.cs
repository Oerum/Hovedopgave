﻿using BoundBot.Application.StaffLicense.Interface;
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

namespace BoundBot.Infrastructure.StaffLicense;

public class StaffLicenseRepository : IStaffLicenseRepository
{
    private readonly ILogger<StaffLicenseRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _ConnectionHandler;

    public StaffLicenseRepository(ILogger<StaffLicenseRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _ConnectionHandler = discordConnectionHandler;
    }


    async Task IStaffLicenseRepository.StaffLicense(SocketSlashCommand command, HttpClient client)
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
                restModel.Model.Hwid = command.GetOptionValues<string>("hwid")!;

                HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/DiscordBot/JwtRefreshAndGenerate", restModel.Model);
                var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                HttpResponseMessage resp = await client.PostAsJsonAsync($"/API/DiscordBot/Command/StaffLicense", restModel.Model);
                var responseBody = await resp.Content.ReadAsStringAsync();
                if (resp.IsSuccessStatusCode)
                {
                    embedBuilder.AddField("Stafflicense",
                        $"\n\n{responseBody}");

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