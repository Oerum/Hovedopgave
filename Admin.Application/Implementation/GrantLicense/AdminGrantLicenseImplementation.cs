using Admin.Application.Interface.GrantLicense;
using Crosscutting;
using Microsoft.Extensions.Logging;

namespace Admin.Application.Implementation.GrantLicense;

public class AdminGrantLicenseImplementation : IAdminGrantLicenseImplementation
{
    private readonly IAdminGrantLicenseRepository _repository;
    private readonly ILogger<AdminGrantLicenseImplementation> _logger;
    public AdminGrantLicenseImplementation(IAdminGrantLicenseRepository repository, ILogger<AdminGrantLicenseImplementation> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    Task<string> IAdminGrantLicenseImplementation.GrantLicense(GrantLicenseDto dto)
    {
        try
        {
            return _repository.GrantLicense(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(1, ex.Message);
            throw new Exception(ex.Message);
        }
    }
}