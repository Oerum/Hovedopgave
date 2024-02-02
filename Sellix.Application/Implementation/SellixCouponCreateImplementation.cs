using Sellix.Application.Interfaces;

namespace Sellix.Application.Implementation;

public class SellixCouponCreateImplementation : ISellixCouponCreateImplementation
{
    private readonly ISellixCouponCreateRepository _repository;

    public SellixCouponCreateImplementation(ISellixCouponCreateRepository repository)
    {
        _repository = repository;
    }

    async Task<string> ISellixCouponCreateImplementation.CreateCoupon(string discordId)
    {
        try
        {
            return await _repository.CreateCoupon(discordId);
        }
        catch (Exception ex)
        {
            throw new Exception("Create Coupon Error", ex);
        }
    }
}