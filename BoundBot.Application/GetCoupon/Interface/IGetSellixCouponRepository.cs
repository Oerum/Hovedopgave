using Discord.WebSocket;

namespace BoundBot.Application.GetCoupon.Interface;

public interface IGetSellixCouponRepository
{
    Task GetCoupon(SocketSlashCommand command, HttpClient client);
}