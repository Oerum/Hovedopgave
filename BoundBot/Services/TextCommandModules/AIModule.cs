using BoundBot.Components.JwtDto;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting;
using Discord.Commands;
using Discord.WebSocket;
using Gpt.Components;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Discord;
using Microsoft.Extensions.Logging;

namespace BoundBot.Services.TextCommandModules;

public class AiModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<AiModule> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _connectionHandler;
    private readonly HttpClient _httpClient;


    public AiModule(IConfiguration configuration, IDiscordConnectionHandler connectionHandler, IHttpClientFactory httpClient, ILogger<AiModule> logger)
    {
        _configuration = configuration;
        _connectionHandler = connectionHandler;
        _logger = logger;
        _httpClient = httpClient.CreateClient("httpClient");
    }

    [Command("!AI")]
    [Summary("Gives an ai response")]
    public async Task AiResponse([Remainder] string question)
    {
        try
        {
            var client = await _connectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

            SocketGuild guild;

            var stopwatch = Stopwatch.StartNew();
            var timeout = TimeSpan.FromSeconds(60);

            do
            {
                guild = client.socketClient.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));

                if (stopwatch.Elapsed >= timeout)
                {
                    break;
                }

            } while (!guild.IsConnected);

            var user = guild.GetUser(Context.Message.Author.Id);

            if (user != null)
            {
                var roles = user.Roles.Select(role => role.Id.ToString()).ToList();
                var usersRoles = roles.Count == 0 ? new List<string>() { "0000000000" } : roles;

                DiscordModelDto model = new()
                {
                    Channel = Context.Channel.Id.ToString(),
                    Command = "AI",
                    Roles = usersRoles,
                    DiscordId = Context.Message.Author.Id.ToString(),
                    DiscordUsername = Context.Message.Author.Username,
                    Hwid = string.Empty,
                    RefreshToken = Context.Message.Author.Id.ToString()
                };


                HttpResponseMessage jwtResponseMessage = await _httpClient.PostAsJsonAsync($"/API/GPT/JwtRefreshAndGenerate", model);
                var jwtResponseBody = await jwtResponseMessage.Content.ReadAsStringAsync();
                var jwtResponseBodyDeserialization = JsonConvert.DeserializeObject<DiscordBotJwtDto>(jwtResponseBody) ?? new DiscordBotJwtDto();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponseBodyDeserialization.AccessToken);
                
                GptModel gpt = new()
                {
                    Question = question
                };

                HttpResponseMessage resp = await _httpClient.PostAsJsonAsync("/API/GPT/AI", gpt);
                var responseBody = await resp.Content.ReadAsStringAsync();

                var embedBuilder = new Discord.EmbedBuilder();
                embedBuilder.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";
                embedBuilder.AddField("AI",
                    $"{Context.Message.Author.Mention} - AI Response Is Not Always Correct!" +
                    $"\n\n**Reponse:**" +
                    $"\n{responseBody}");
                embedBuilder.WithColor(Color.DarkOrange);
                embedBuilder.WithCurrentTimestamp();

                await ReplyAsync(embed: embedBuilder.Build());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Command Error: {ErrorMessage}", ex.Message);

        }
    }
}