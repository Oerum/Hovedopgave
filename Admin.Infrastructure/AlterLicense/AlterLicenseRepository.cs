using Admin.Application.Interface.GrantLicense;
using Admin.Infrastructure.GrantLicense;
using Auth.Database.Model;
using Crosscutting;
using Microsoft.Extensions.Logging;
using Admin.Application.Interface.AlterLicense;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Auth.Database.Contexts;

namespace Admin.Infrastructure.IAlterLicense
{
    public class AlterLicenseRepository : IAlterLicenseRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger<AlterLicenseRepository> _logger;

        public AlterLicenseRepository(AuthDbContext db, ILogger<AlterLicenseRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        async Task<string> IAlterLicenseRepository.AlterLicense(AlterLicenseDTO dto)
        {
            try
            {
                _logger.LogInformation(1, "Altering License For: {0}", dto.DiscordId);

                var license = await _db.ActiveLicenses.Include(x => x.User).Include(y => y.Order).ToListAsync();

                var toUpdate = license?.FirstOrDefault(x => x.Order.UniqId == dto.OrderId);

                if (toUpdate != null)
                {
                    toUpdate.User.DiscordId = dto.DiscordId!;
                    toUpdate.User.DiscordUsername = dto.DiscordName!;

                    if (dto.Product != null)
                    {
                        toUpdate.ProductNameEnum = (WhichSpec)(int)dto.Product;
                    }

                    await _db.SaveChangesAsync();

                    return $"License: {toUpdate.Order.UniqId} has been sucessfully altered!" +
                        $"\nUser: {dto.DiscordName} : {dto.DiscordId}" +
                        $"\nProduct: {dto.Product}";
                }
                else
                {
                    return $"No license(s) found with given id: {dto.OrderId}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
