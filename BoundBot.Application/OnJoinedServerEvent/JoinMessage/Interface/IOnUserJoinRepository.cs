using Discord.WebSocket;

namespace BoundBot.Application.OnJoinedServerEvent.JoinMessage.Interface;

public interface IOnUserJoinRepository
{
    Task UserJoined(SocketGuildUser user);
}