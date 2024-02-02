using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.AdminCheckLicenses.Implementation;

public class AdminCheckLicensesImplementation : IAdminCheckLicensesImplementation
{
    private readonly ILogger<AdminCheckLicensesImplementation> _logger;
    private readonly IDomainRoleCheck _roleCheck;
    private readonly IAdminCheckLicensesRepository _checkLicensesRepository;
    public AdminCheckLicensesImplementation(ILogger<AdminCheckLicensesImplementation> logger, IDomainRoleCheck roleCheck, IAdminCheckLicensesRepository checkLicensesRepository)
    {
        _logger = logger;
        _roleCheck = roleCheck;
        _checkLicensesRepository = checkLicensesRepository;
    }


    async Task IAdminCheckLicensesImplementation.CheckLicenses(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundStaffAndAbove);

            if (access)
            {
                await _checkLicensesRepository.CheckLicenses(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}