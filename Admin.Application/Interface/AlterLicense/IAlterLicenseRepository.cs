using Crosscutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Interface.AlterLicense
{
    public interface IAlterLicenseRepository
    {
        public Task<string> AlterLicense(AlterLicenseDTO dto);
    }
}
