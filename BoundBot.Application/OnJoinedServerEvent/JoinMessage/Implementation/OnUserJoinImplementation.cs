using BoundBot.Application.OnJoinedServerEvent.JoinMessage.Interface;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.OnJoinedServerEvent.JoinMessage.Implementation;

public class OnUserJoinImplementation : IOnUserJoinImplementation
{
    private readonly ILogger<OnUserJoinImplementation> _logger;
    private readonly IOnUserJoinRepository _repository;

    public OnUserJoinImplementation(ILogger<OnUserJoinImplementation> logger, IOnUserJoinRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    async Task IOnUserJoinImplementation.UserJoined(SocketGuildUser user)
    {
        try
        {
            await _repository.UserJoined(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}