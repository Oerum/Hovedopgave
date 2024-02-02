namespace Sellix.Application.Interfaces;

public interface ISellixCouponCreateImplementation
{
    Task<string> CreateCoupon(string discordId);
}