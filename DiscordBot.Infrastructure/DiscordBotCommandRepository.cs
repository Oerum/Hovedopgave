using Auth.Database;
using Auth.Database.Model;
using Crosscutting;
using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using BoundBot.Connection.DiscordConnectionHandler;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using BoundBot.Components.Members;
using Discord.Rest;
using static BoundBot.Components.Members.DiscordServerMembersHandler;

namespace DiscordBot.Infrastructure
{
    public class DiscordBotCommandRepository : IDiscordBotCommandRepository
    {
        private readonly ILogger _logger;
        private readonly AuthDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IDiscordConnectionHandler _connectionHandler;
        private readonly IDiscordServerMembersHandler _serverMembersHandler;

        public DiscordBotCommandRepository(ILogger<DiscordBotCommandRepository> logger, AuthDbContext db, IConfiguration configuration, IDiscordConnectionHandler connectionHandler, IDiscordServerMembersHandler serverMembersHandler)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration;
            _connectionHandler = connectionHandler;
            _serverMembersHandler = serverMembersHandler;
        }

        async Task<string> IDiscordBotCommandRepository.GetStaffLicense(DiscordModelDto model)
        {
            try
            {
                _logger.LogInformation(model.DiscordUsername + " Engaged GetStaffLicense At: " + DateTime.UtcNow);

                var check = await _db.ActiveLicenses.Include(user => user.User).Include(order => order.Order).Where(x => x.User.DiscordId == model.DiscordId || x.User.DiscordUsername == model.DiscordUsername).ToListAsync();
                var userExists = await _db.User!.FirstOrDefaultAsync(x => x.DiscordId == model.DiscordId && x.DiscordUsername == model.DiscordUsername);

                if (check.Any())
                {
                    foreach (var item in check)
                    {
                        if (item.ProductName == "STAFF")
                        {
                            throw new Exception($"{model.DiscordUsername} already has attached stafflicense!");
                        }
                    }
                }

                string dbUserId;

                if (userExists == null)
                {
                    var user = new UserDbModel
                    {
                        Email = model.DiscordUsername,
                        Firstname = model.DiscordUsername,
                        Lastname = model.DiscordUsername,
                        DiscordUsername = model.DiscordUsername,
                        DiscordId = model.DiscordId,
                        HWID = model.Hwid,
                    };

                    await _db.User.AddAsync(user);
                    await _db.SaveChangesAsync();

                    dbUserId = user.UserId;
                }
                else
                {
                    dbUserId = userExists.UserId;
                }

                var Order = new OrderDbModel
                {
                    UserId = dbUserId,
                    UniqId = "STAFF",
                    ProductName = "STAFF",
                    ProductPrice = "STAFF",
                    PurchaseDate = DateTime.UtcNow,
                };

                await _db.Order.AddAsync(Order);
                await _db.SaveChangesAsync();

                var Licenses = new ActiveLicensesDbModel
                {
                    ProductName = "STAFF",
                    ProductNameEnum = WhichSpec.AIO,
                    EndDate = DateTime.UtcNow.AddDays(365),
                    UserId = dbUserId,
                    OrderId = Order.OrderId
                };

                await _db.ActiveLicenses.AddAsync(Licenses);
                await _db.SaveChangesAsync();

                return await Task.FromResult("Stafflicense Has been created:" +
                    $"\nName    : {Licenses.ProductName}" +
                    $"\nType    : {Licenses.ProductNameEnum}" +
                    $"\nEnd     : {Licenses.EndDate}" +
                    $"\nUserId  : {Licenses.UserId}" +
                    $"\nOrderId : {Licenses.OrderId}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async Task<string> IDiscordBotCommandRepository.UpdateDiscordAndRole(DiscordModelDto model)
        {
            try
            {
                var client = await _connectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);

                var discordUserId = ulong.Parse(model.DiscordId);
                var guildId = ulong.Parse(_configuration["Discord:Guid"] ?? string.Empty);

                _logger.LogInformation(1, $"{model.DiscordUsername} Engaged UpdateDiscord At: {DateTime.UtcNow}");

                var activeLicenses = await _db.ActiveLicenses
                    .Include(user => user.User)
                    .Where(x => x.User.DiscordId == model.DiscordId || x.User.DiscordUsername == model.DiscordUsername)
                    .ToListAsync();


                int activeLicensesCount = activeLicenses.Count(item => DateTime.UtcNow < item.EndDate);

                bool userAlreadyHasRole = false;

                switch (activeLicensesCount)
                {
                    case 0:
                        var socketGuild = client.socketClient.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));

                        var userExe = socketGuild.GetUser(ulong.Parse(model.DiscordId));

                        throw new Exception($"{userExe.Mention} you do not have active license(s)");

                    case > 0:
                        var restGuild = await client.restClient.GetGuildAsync(ulong.Parse(_configuration["Discord:Guid"]!));

                        var restGuildUser = await client.restClient.GetGuildUserAsync(guildId, Convert.ToUInt64(model.DiscordId)); // Get a SocketUser

                        if (restGuildUser != null && restGuild != null)
                        {
                            foreach (var item in activeLicenses)
                            {
                                item.User.DiscordId = model.DiscordId;
                                item.User.DiscordUsername = model.DiscordUsername;

                                ulong.TryParse(_configuration["Discord:Role:AIO"], out ulong aioResult);
                                ulong.TryParse(_configuration["Discord:Role:Month"], out ulong monthResult);

                                ulong roleId = (ulong)(item.ProductNameEnum == WhichSpec.AIO ? aioResult : monthResult);

                                try
                                {
                                    var role = restGuild.GetRole(roleId);

                                    if (role != null)
                                    {
                                        if (roleId == aioResult)
                                        {
                                            if (!model.Roles!.Contains(roleId.ToString()))
                                            {
                                                await restGuildUser!.AddRoleAsync(role);
                                                await restGuildUser.UpdateAsync();
                                            }
                                            else
                                            {
                                                userAlreadyHasRole = true;
                                            }
                                        }
                                        else if (roleId == monthResult)
                                        {
                                            try
                                            {
                                                if (!model.Roles!.Contains(monthResult.ToString()))
                                                {
                                                    await restGuildUser!.AddRoleAsync(role);
                                                    await restGuildUser.UpdateAsync();
                                                }
                                                else
                                                {
                                                    userAlreadyHasRole = true;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new Exception($"Unable to grant role @ Reason [@Admin]: {ex.Message}");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"Unable to grant role @ Reason [@Admin]: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("GuildUser or Guild is null.");
                            throw new Exception("Unable to grant role @ Admin");
                        }

                        await _db.SaveChangesAsync();
                        break;
                }

                switch (userAlreadyHasRole)
                {
                    case true:
                        return "You already have correct roles based on your active license(s)";

                    case false:
                        return "Successfully Updated Account & Roles";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        async Task<string> IDiscordBotCommandRepository.UpdateHwid(DiscordModelDto model)
        {
            try
            {
                _logger.LogInformation(model.DiscordId + " Engaged UpdateHwid At: " + DateTime.UtcNow);

                var check = await _db.ActiveLicenses.Include(user => user.User).Include(order => order.Order).Where(x => x.User.DiscordId == model.DiscordId || x.User.DiscordUsername == model.DiscordUsername).ToListAsync();

                //if (!check.Any()) { throw new Exception($"{model.DiscordUsername} doesn't exist in the database"); }

                int activeLicenses = 0;

                foreach (var item in check)
                {
                    if (item.EndDate < DateTime.UtcNow) { activeLicenses++; }
                }


                StringBuilder builder = new();

                if (activeLicenses >= 0)
                {
                    foreach (var item in check)
                    {
                        item.User.HWID = model.Hwid;

                        builder.AppendLine(item.Order.UniqId);

                    }
                    await _db.SaveChangesAsync();
                }

                //if (!check.Any()) { throw new Exception($"{model.DiscordUsername} doesn't exist in the database"); }
                
                await _db.SaveChangesAsync();

                if (builder.Length == 0) { throw new Exception($"Account: [{model.DiscordUsername} | {model.DiscordId}] doesn't have an active license"); } //Remove This

                return await Task.FromResult("Hwid & Roles Updated License(s):" +
                    $"\n{builder}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
