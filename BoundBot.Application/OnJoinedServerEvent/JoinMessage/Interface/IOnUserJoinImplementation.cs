using Discord.WebSocket;

namespace BoundBot.Application.OnJoinedServerEvent.JoinMessage.Interface;

public interface IOnUserJoinImplementation
{
    Task UserJoined(SocketGuildUser user);
}