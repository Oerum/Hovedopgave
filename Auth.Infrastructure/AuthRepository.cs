using Auth.Application.Interface;
using Auth.Database;
using Crosscutting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger _logger;
        public AuthRepository(AuthDbContext db, ILogger<AuthRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        async Task<List<AuthModelDTO>> IAuthRepository.Auth(AuthModelDTO model)
        {
            try
            {
                _logger.LogInformation(model.HWID + " Engaged Auth At: " + DateTime.UtcNow);

                var returnList = new List<AuthModelDTO>();

                var auth = await _db.ActiveLicenses
                    .Include(user => user.User)
                    .Where(user => user.User.HWID == model.HWID && DateTime.UtcNow < user.EndDate)
                    .ToListAsync();

                _logger.LogInformation($"HWID: {model.HWID} | Name: {auth.FirstOrDefault()?.User.Firstname} {auth.FirstOrDefault()?.User.Lastname} | Succesfully authed at: {DateTime.UtcNow}");

                if (auth.Count != 0)
                {
                    var authList = auth.Select(ele => new AuthModelDTO
                    {
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
                    })
                    .ToList();

                    // Log the end dates here
                    foreach (var authItem in authList)
                    {
                        _logger.LogInformation(1, null, $"End Date: {authItem.EndDate}\n UTC: {DateTime.UtcNow}");
                    }

                    returnList.AddRange(authList);
                }

                return [.. returnList.OrderByDescending(x => x.EndDate)];
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, "Auth Error!");
                throw new Exception(ex.Message);
            }
        }
    }
}