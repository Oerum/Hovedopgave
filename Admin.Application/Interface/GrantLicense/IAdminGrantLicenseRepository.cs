using Crosscutting;

namespace Admin.Application.Interface.GrantLicense;

public interface IAdminGrantLicenseRepository
{
    public Task<string> GrantLicense(GrantLicenseDto dto);
}