using BoundBot.Application.Gpt.Interface;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.Gpt.Implementation;

public class GptDiscordImplementation : IGptDiscordImplementation
{
    private readonly ILogger<GptDiscordImplementation> _logger;
    private readonly IGptDiscordRepository _gpt;

    public GptDiscordImplementation(ILogger<GptDiscordImplementation> logger, IGptDiscordRepository gpt)
    {
        _logger = logger;
        _gpt = gpt;
    }

    async Task IGptDiscordImplementation.UpdateFtModel(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            await _gpt.UpdateFtModel(command, client);
        }
        catch (Exception ex)
        {
            _logger.LogError("Discord Request Update AI Model", ex);
            throw new Exception(ex.Message);
        }
    }
}