using BoundBot.Application.GetCoupon.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.GetCoupon.Implementation;

public class GetSellixCouponImplementation : IGetSellixCouponImplementation
{
    private readonly ILogger<GetSellixCouponImplementation> _logger;
    private readonly IGetSellixCouponRepository _repository;
    private readonly IDomainRoleCheck _roleCheck;

    public GetSellixCouponImplementation(ILogger<GetSellixCouponImplementation> logger, IGetSellixCouponRepository repository, IDomainRoleCheck roleCheck)
    {
        _logger = logger;
        _repository = repository;
        _roleCheck = roleCheck;
    }


    async Task IGetSellixCouponImplementation.GetCoupon(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundServerBooster);

            if (access)
            {
                await _repository.GetCoupon(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}