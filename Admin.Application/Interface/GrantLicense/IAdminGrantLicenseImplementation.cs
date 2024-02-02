using Crosscutting;

namespace Admin.Application.Interface.GrantLicense;

public interface IAdminGrantLicenseImplementation
{
    public Task<string> GrantLicense(GrantLicenseDto dto);
}