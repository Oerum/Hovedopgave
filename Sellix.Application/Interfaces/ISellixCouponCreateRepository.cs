namespace Sellix.Application.Interfaces;

public interface ISellixCouponCreateRepository
{
    Task<string> CreateCoupon(string discordId);
}