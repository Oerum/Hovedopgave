using Discord.WebSocket;

namespace BoundBot.Application.GetCoupon.Interface;

public interface IGetSellixCouponImplementation
{
    Task GetCoupon(SocketSlashCommand command, HttpClient client);
}