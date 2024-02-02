using Admin.Application.Implementation.GrantLicense;
using Admin.Application.Interface.AlterLicense;
using Admin.Application.Interface.GrantLicense;
using Crosscutting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Implementation.AlterLicense
{
    public class AlterLicenseImplementation : IAlterLicenseImplementation
    {
        private readonly IAlterLicenseRepository _repository;
        private readonly ILogger<AlterLicenseImplementation> _logger;
        public AlterLicenseImplementation(IAlterLicenseRepository repository, ILogger<AlterLicenseImplementation> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        Task<string> IAlterLicenseImplementation.AlterLicense(AlterLicenseDTO dto)
        {
            try
            {
                return _repository.AlterLicense(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
