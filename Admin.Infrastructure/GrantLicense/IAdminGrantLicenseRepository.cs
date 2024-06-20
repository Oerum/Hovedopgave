using Admin.Application.Interface.GrantLicense;
using Auth.Database.Contexts;
using Auth.Database.Model;
using Crosscutting;
using Crosscutting.SellixPayload;
using DiscordSaga.Components.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Admin.Infrastructure.GrantLicense;

public class AdminGrantLicenseRepository : IAdminGrantLicenseRepository
{
    private readonly AuthDbContext _db;
    private readonly ILogger<AdminGrantLicenseRepository> _logger;

    public AdminGrantLicenseRepository(AuthDbContext db, ILogger<AdminGrantLicenseRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    async Task<string> IAdminGrantLicenseRepository.GrantLicense(GrantLicenseDto dto)
    {
            try
            {
                    var userExists = await _db.User!.FirstOrDefaultAsync(x =>
                        x.DiscordId == dto.DiscordId);

                    _logger.LogInformation(1,"Granting License For: {0}", dto.DiscordId);

                    DateTime time = DateTime.UtcNow;
                    WhichSpec? whichSpec = dto.Product;

                    if (dto.Product == WhichSpec.AIO)
                    {
                        whichSpec = WhichSpec.AIO;
                        var currentLicenseTime = await _db.ActiveLicenses.Include(user => user.User)
                            .Include(order => order.Order)
                            .Where(x => (x.User.DiscordId == dto.DiscordId ||
                                         x.User.DiscordUsername ==dto.DiscordUsername) &&
                                        x.ProductNameEnum == WhichSpec.AIO).ToListAsync() ?? null;

                        if (!currentLicenseTime.IsNullOrEmpty())
                        {
                            if (currentLicenseTime != null)
                            {
                                var maxCurrentLicenseTime = currentLicenseTime.MaxBy(x => x.EndDate);

                                if (maxCurrentLicenseTime?.EndDate >= DateTime.UtcNow)
                                {
                                    time = currentLicenseTime.MaxBy(x => x.EndDate)!.EndDate;
                                }

                                _db.ActiveLicenses.RemoveRange(currentLicenseTime);
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {

                        var currentMonthlyLicenseTime = await _db.ActiveLicenses.Include(user => user.User)
                            .Where(x => (x.User.DiscordId == dto.DiscordId ||
                                         x.User.DiscordUsername == dto.DiscordUsername) &&
                                        x.ProductNameEnum == whichSpec).ToListAsync() ?? null;

                        if (!currentMonthlyLicenseTime.IsNullOrEmpty())
                        {
                            if (currentMonthlyLicenseTime != null)
                            {
                                var maxCurrentMonthlyLicenseTime = currentMonthlyLicenseTime.MaxBy(x => x.EndDate);

                                if (maxCurrentMonthlyLicenseTime?.EndDate >= DateTime.UtcNow)
                                {
                                    time = maxCurrentMonthlyLicenseTime.EndDate;
                                }

                                _db.ActiveLicenses.RemoveRange(currentMonthlyLicenseTime);
                                await _db.SaveChangesAsync();
                            }
                        }
                    }

                    string dbUserId;

                    if (userExists == null)
                    {
                        var user = new UserDbModel
                        {
                            Email = dto.DiscordUsername + "@grant-license.com",
                            Firstname = dto.DiscordUsername ?? "Unknown",
                            Lastname = dto.DiscordUsername ?? "Unknown_Sur",
                            DiscordUsername = dto.DiscordUsername ?? "GrantLicense",
                            DiscordId = dto.DiscordId ?? "GrantLicense",
                            HWID = dto.Hwid ?? "Missing"
                        };

                        await _db.User.AddAsync(user);
                        await _db.SaveChangesAsync();

                        dbUserId = user.UserId;
                    }
                    else
                    {
                        dbUserId = userExists.UserId;

                        if (userExists.DiscordUsername != dto.DiscordUsername && dto.DiscordUsername != null)
                        {
                            userExists.DiscordUsername = dto.DiscordUsername;
                            await _db.SaveChangesAsync();
                        }
                    }

                    var order = new OrderDbModel
                    {
                        UserId = dbUserId,
                        UniqId = Guid.NewGuid().ToString(),
                        ProductName = "License Grant",
                        ProductPrice = "0.00",
                        PurchaseDate = DateTime.UtcNow
                    };

                    await _db.Order!.AddAsync(order);
                    await _db.SaveChangesAsync();

                    var licenses = new ActiveLicensesDbModel
                    {
                        UserId = dbUserId,
                        ProductName = "License Grant",
                        ProductNameEnum = (WhichSpec)whichSpec!,
                        EndDate = time.AddMinutes(Convert.ToInt32(dto.MinutesToExtend)),
                        OrderId = order.OrderId
                    };

                    await _db.ActiveLicenses!.AddAsync(licenses);
                    await _db.SaveChangesAsync();

                    return $"User: {dto.DiscordUsername} Id: {dto.DiscordId}" +
                           $"\nProduct: {(WhichSpec)whichSpec}" +
                           $"\nEndDate: {time.AddMinutes(Convert.ToInt32(dto.MinutesToExtend))} UTC";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
    }
}