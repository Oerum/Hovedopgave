namespace Admin.Application.Interface.ExtendLicense;

public interface IAdminExtendLicensesImplementation
{
    Task<string> ExtendLicense(int minutesToExtend, string? discordId = null);
}