namespace Admin.Application.Interface.ExtendLicense;

public interface IAdminExtendLicensesRepository
{
    Task<string> ExtendLicense(int minutesToExtend, string? discordId = null);
}