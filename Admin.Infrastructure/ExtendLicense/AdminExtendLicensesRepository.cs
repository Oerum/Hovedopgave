using Admin.Application.Interface.ExtendLicense;
using Auth.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Admin.Infrastructure.ExtendLicense;

public class AdminExtendLicensesRepository : IAdminExtendLicensesRepository
{

    private readonly AuthDbContext _authDbContext;
    private readonly ILogger<AdminExtendLicensesRepository> _logger;

    public AdminExtendLicensesRepository(AuthDbContext authDbContext, ILogger<AdminExtendLicensesRepository> logger)
    {
        _authDbContext = authDbContext;
        _logger = logger;
    }


    async Task<string> IAdminExtendLicensesRepository.ExtendLicense(int minutesToExtend, string? discordId)
    {
        try
        {
            var activeLicenses = await _authDbContext.ActiveLicenses.Include(x => x.User).ToListAsync();

            if (discordId != null)
            {
                foreach (var license in activeLicenses.Where(x => x.User.DiscordId == discordId))
                {
                    license.EndDate = license.EndDate.AddMinutes(minutesToExtend);
                }

                await _authDbContext.SaveChangesAsync();

                _logger.LogInformation($"Licenses for user: {discordId} extended by: {minutesToExtend} minute(s)");
                return $"Licenses for user: {discordId} extended by: {minutesToExtend} minute(s)";
            }
            else
            {
                foreach (var license in activeLicenses)
                {
                    license.EndDate = license.EndDate.AddMinutes(minutesToExtend);
                }

                await _authDbContext.SaveChangesAsync();

                _logger.LogInformation($"All active licenses extended by: {minutesToExtend} minute(s)");
                return $"All active licenses extended by: {minutesToExtend} minute(s)";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("License Extension Error", ex.Message);
            return ex.Message;
        }
    }
}