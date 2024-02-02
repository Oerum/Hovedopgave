using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscutting
{
    public class AlterLicenseDTO
    {
        public string? OrderId { get; set; }
        public WhichSpec? Product { get; set; } = 0;
        public string? ProdcutName { get; set; }
        public string? DiscordId { get; set; } = string.Empty;
        public string? DiscordName { get; set; } = string.Empty;
    }
}
