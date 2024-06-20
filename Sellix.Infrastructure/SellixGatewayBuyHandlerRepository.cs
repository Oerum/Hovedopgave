using Auth.Database.Contexts;
using Auth.Database.Model;
using Crosscutting;
using Crosscutting.SellixPayload;
using DiscordSaga.Components.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sellix.Application.Interfaces;

namespace Sellix.Infrastructure
{
    public class SellixGatewayBuyHandlerRepository : ISellixGatewayBuyHandlerRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger<SellixGatewayBuyHandlerRepository> _logger;

        public SellixGatewayBuyHandlerRepository(AuthDbContext db, ILogger<SellixGatewayBuyHandlerRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        async Task<LicenseNotificationEvent> ISellixGatewayBuyHandlerRepository.OrderHandler(SellixPayloadNormal.Root root)
        {
            try
            {
                if (root.Event is "order:paid" or "order:paid:product" &&
                    root.Data.StatusHistory[0].InvoiceId != "dummy")
                {
                    var userExists = await _db.User!.FirstOrDefaultAsync(x =>
                        x.DiscordId == root.Data.CustomFields.DiscordId) ?? null;

                    _logger.LogInformation(root.Data.CustomFields.DiscordId + " " + root.Data.CustomFields.DiscordUser +
                                           " @ Engaged purchase at: " + DateTime.UtcNow);


                    List<int> quantityList = new();
                    List<DateTime> timeList = new();
                    List<WhichSpec> specList = new();

                    int productCount = root.Data.Products.Count;

                    foreach (var product in root.Data.Products)
                    {
                        DateTime time = DateTime.UtcNow;
                        WhichSpec whichSpec = WhichSpec.none;
                        int quantity = 0;
                        string productTitle = "";

                        if (product.Title.Contains("AIO", StringComparison.CurrentCultureIgnoreCase)
                            || product.Title.Contains("Season of Discovery", StringComparison.CurrentCultureIgnoreCase))
                        {
                            whichSpec = product.Title switch
                            {
                                "AIO" => WhichSpec.AIO,
                                _ => WhichSpec.none
                            };

                            specList.Add(whichSpec);
                            quantity = Convert.ToInt32(productCount == 1 ? root.Data.Quantity : product.UnitQuantity);
                            quantityList.Add(quantity);
                            var currentLicenseTime = await _db.ActiveLicenses.Include(user => user.User)
                                .Include(order => order.Order)
                                .Where(x => (x.User.DiscordId == root.Data.CustomFields.DiscordId) &&
                                            x.ProductNameEnum == WhichSpec.AIO).ToListAsync() ?? null;

                            if (currentLicenseTime != null && currentLicenseTime.Any())
                            {
                                var maxCurrentLicenseTime = currentLicenseTime.MaxBy(x => x.EndDate);

                                if (maxCurrentLicenseTime?.EndDate >= DateTime.UtcNow)
                                {
                                    time = currentLicenseTime.MaxBy(x => x.EndDate)!.EndDate;
                                }

                                _db.ActiveLicenses.RemoveRange(currentLicenseTime);
                                await _db.SaveChangesAsync();
                            }

                            productTitle = product.Title;
                            timeList.Add(time.AddDays(Convert.ToInt32(1 * quantity)));
                        }
                        else
                        {
                            whichSpec = product.Title switch
                            {
                                "Placeholder" => WhichSpec.Placeholder,
                                _ => 0
                            };

                            specList.Add(whichSpec);

                            quantity = 30 * Convert.ToInt32(productCount == 1 ? root.Data.Quantity : product.UnitQuantity);
                            quantityList.Add(quantity);

                            var currentMonthlyLicenseTime = await _db.ActiveLicenses.Include(user => user.User)
                                .Where(x => (x.User.DiscordId == root.Data.CustomFields.DiscordId) &&
                                            x.ProductNameEnum == whichSpec).ToListAsync() ?? null;

                            if (currentMonthlyLicenseTime != null && currentMonthlyLicenseTime.Any())
                            {
                                var maxCurrentMonthlyLicenseTime = currentMonthlyLicenseTime.MaxBy(x => x.EndDate);

                                if (maxCurrentMonthlyLicenseTime?.EndDate >= DateTime.UtcNow)
                                {
                                    time = maxCurrentMonthlyLicenseTime.EndDate;
                                }

                                _db.ActiveLicenses.RemoveRange(currentMonthlyLicenseTime);
                                await _db.SaveChangesAsync();
                            }

                            productTitle = product.Title;
                            timeList.Add(time.AddDays(Convert.ToInt32(1 * quantity)));
                        }

                        string dbUserId;

                        if (root.Data.CouponCode != "19375IAmSuperSecretDeveloper19375")
                        {
                            if (userExists == null)
                            {
                                var user = new UserDbModel
                                {
                                    Email = root.Data.CustomerEmail,
                                    Firstname = root.Data.CustomFields.AcfName ?? root.Data.CustomFields.DiscordUser,
                                    Lastname = root.Data.CustomFields.AcfSurname ?? root.Data.CustomFields.DiscordUser,
                                    DiscordUsername = root.Data.CustomFields.DiscordUser,
                                    DiscordId = root.Data.CustomFields.DiscordId,
                                    HWID = root.Data.CustomFields.HWID ?? "Deprecated"
                                };

                                await _db.User.AddAsync(user);
                                userExists = user;

                                dbUserId = user.UserId;
                            }
                            else
                            {
                                dbUserId = userExists.UserId;

                                userExists.Email = root.Data.CustomerEmail;
                                userExists.DiscordUsername = root.Data.CustomFields.DiscordUser;
                                userExists.DiscordId = root.Data.CustomFields.DiscordId;
                                userExists.HWID = root.Data.CustomFields.HWID ?? "Deprecated";

                                if (userExists.Firstname == root.Data.CustomFields.DiscordUser || userExists.Lastname == root.Data.CustomFields.DiscordUser)
                                {
                                    userExists.Firstname = root.Data.CustomFields.AcfName ?? root.Data.CustomFields.DiscordUser;
                                    userExists.Lastname = root.Data.CustomFields.AcfSurname ?? root.Data.CustomFields.DiscordUser;
                                }
                            }

                            var order = new OrderDbModel
                            {
                                UserId = dbUserId,
                                UniqId = root.Data.Uniqid,
                                ProductName = productTitle ?? "Unknown",
                                ProductPrice = root.Data.TotalDisplay.ToString() ?? "null",
                                PurchaseDate = DateTime.UtcNow
                            };

                            await _db.Order!.AddAsync(order);

                            var licenses = new ActiveLicensesDbModel
                            {
                                UserId = dbUserId,
                                ProductName = productTitle ?? "Unknown",
                                ProductNameEnum = whichSpec,
                                EndDate = time.AddDays(Convert.ToInt32(1 * quantity)),
                                OrderId = order.OrderId
                            };

                            await _db.ActiveLicenses!.AddAsync(licenses);
                        }
                    }

                    await _db.SaveChangesAsync();

                    var serializePayload = JsonConvert.SerializeObject(root);

                    var message = new LicenseNotificationEvent
                    {
                        Payload = serializePayload,
                        Quantity = quantityList,
                        Time = timeList,
                        WhichSpec = specList,
                    };

                    return message;
                }
                else
                {
                    throw new Exception("Dummy or order not paid.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            throw new Exception("Unknown Error In OrderHandler");
        }
    }
}
