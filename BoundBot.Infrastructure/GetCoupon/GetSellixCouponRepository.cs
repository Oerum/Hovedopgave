﻿using BoundBot.Application.GetCoupon.Interface;
using BoundBot.Components.JwtDto;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace BoundBot.Infrastructure.GetCoupon;

public class GetSellixCouponRepository : IGetSellixCouponRepository
{
    private readonly ILogger<GetSellixCouponRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _ConnectionHandler;

    public GetSellixCouponRepository(ILogger<GetSellixCouponRepository> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _ConnectionHandler = discordConnectionHandler;
    }


    async Task IGetSellixCouponRepository.GetCoupon(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await command.DeferAsync(false);

            DiscordModelDtoRestModel restModel = new(command);
            var embedBuilder = new Discord.EmbedBuilder
            {
                ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png"
            };

            if (restModel.Model.Roles!.Contains(_configuration["Discord:Role:Boost"]!))
            {
                HttpResponseMessage jwtResponseMessage = await client.PostAsJsonAsync($"/API/Sellix/JwtRefreshAndGenerate", restModel.Model);
                var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);

                var payload = new JsonObject
                {
                    ["discord_id"] = restModel.Model.DiscordId
                };

                HttpResponseMessage resp = await client.PostAsJsonAsync($"/API/Sellix/Command/CreateCoupon", payload);

                var responseBody = await resp.Content.ReadAsStringAsync();


                if (resp.IsSuccessStatusCode)
                {
                    embedBuilder.AddField("Coupon",
                        $"\n{responseBody}");

                    embedBuilder.WithColor(Color.DarkOrange);
                }
                else
                {
                    embedBuilder.AddField("BadRequest", responseBody);
                }
            }
            else
            {
                embedBuilder.AddField("Denied", "You're not a server booster!");
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