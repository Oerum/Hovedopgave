using Auth.Application.Interface;
using Auth.Components;
using Auth.Database.Contexts;
using Auth.Database.Model;
using Crosscutting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Auth.Infrastructure
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger _logger;
        private readonly DiscordOAuthContext discordOAuth;

        public AuthRepository(AuthDbContext db, ILogger<AuthRepository> logger, DiscordOAuthContext discordOAuth)
        {
            _db = db;
            _logger = logger;
            this.discordOAuth = discordOAuth;
        }

        async Task<List<AuthModelDTO>> IAuthRepository.Auth(AuthModelDTO model)
        {
            try
            {
                if (model.AuthRequest == WhichSpec.none)
                {
                    _logger.LogWarning($"AuthRequest cannot be default @ {model.AuthRequest}, {model.ProductNameEnum}");
                    throw new UnauthorizedAccessException("AuthRequest cannot be default @ throw");
                }

                var authList = new List<AuthModelDTO>();

                var activeAuthLicenses = await _db.ActiveLicenses
                    .Include(user => user.User)
                    .Where(user => (user.User.HWID == model.HWID || user.User.DiscordId == model.DiscordId) && DateTime.UtcNow < user.EndDate)
                    .ToListAsync();

                var firstName = activeAuthLicenses.FirstOrDefault()?.User.Firstname;
                var lastName = activeAuthLicenses.FirstOrDefault()?.User.Lastname;

                _logger.LogInformation($"Succesfully authed at: {DateTime.UtcNow} by: {firstName} {lastName} | {model.DiscordUsername} : {model.DiscordId} | {model.Email}");

                if (activeAuthLicenses.Count != 0)
                {
                    foreach (var ele in activeAuthLicenses.OrderByDescending(x => x.EndDate))
                    {
                        var productName = ele.ProductName.ToLower();
                        var productNameEnum = ele.ProductNameEnum;

                        if (productName.Contains("aio", StringComparison.CurrentCultureIgnoreCase)
                            || productNameEnum == WhichSpec.AIO
                            || productName == "staff"
                            || productName == "staff"
                            || (Enum.GetName(typeof(WhichSpec), model.AuthRequest)!.Contains("placeholder", StringComparison.CurrentCultureIgnoreCase)
                                && (productName.Contains("placeholder")
                                || productNameEnum == WhichSpec.Placeholder))
                            || (model.AuthRequest == WhichSpec.Placeholder && (productName == "Placeholder" || productNameEnum == WhichSpec.Placeholder)))
                        {
                            authList.Add(new AuthModelDTO
                            {
                                Success = true,
                                Email = ele.User.Email,
                                Firstname = ele.User.Firstname,
                                Lastname = ele.User.Lastname,
                                DiscordUsername = ele.User.DiscordUsername,
                                DiscordId = ele.User.DiscordId,
                                HWID = ele.User.HWID,
                                ProductName = ele.ProductName,
                                EndDate = ele.EndDate,
                                UserId = ele.UserId,
                                ProductNameEnum = ele.ProductNameEnum
                            });
                        }
                        else
                        {
                            authList.Add(new AuthModelDTO
                            {
                                Success = false,
                                Email = ele.User.Email,
                                Firstname = ele.User.Firstname,
                                Lastname = ele.User.Lastname,
                                DiscordUsername = ele.User.DiscordUsername,
                                DiscordId = ele.User.DiscordId,
                                HWID = ele.User.HWID,
                                ProductName = ele.ProductName,
                                EndDate = ele.EndDate,
                                UserId = ele.UserId,
                                ProductNameEnum = ele.ProductNameEnum
                            });
                        }
                    }
                }

                return [.. authList.OrderByDescending(x => x.EndDate)];
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, "Auth Error!");
                throw;
            }
        }

        async Task<DiscordOAuthModel> IAuthRepository.DiscordOAuth(DiscordOAuthDTO model)
        {
            try
            {
                var result = await discordOAuth.OAuth.FirstOrDefaultAsync(x=>x.State == model.State);

                foreach (var ele in discordOAuth.OAuth)
                {
                    if (ele.Expires_at <= DateTime.UtcNow)
                    {
                        discordOAuth.OAuth.Remove(ele);
                    }
                }

                await discordOAuth.SaveChangesAsync();

                if (result == null)
                {
                    _logger.LogWarning($"DiscordOAuth Error!\nNo Auth users with given state: {model.State}");
                    throw new UnauthorizedAccessException("No Auth users with given state");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(1, ex, "DiscordOAuth Error!");
                throw;
            }
        }
    }
}