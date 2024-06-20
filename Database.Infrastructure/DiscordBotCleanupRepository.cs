using Auth.Database.Contexts;
using BoundBot.Connection.DiscordConnectionHandler;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.TransactionHandling;
using Database.Application.Interface;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Database.Infrastructure;

public class DiscordBotCleanupRepository : IDiscordBotCleanupRepository
{
    private readonly AuthDbContext _db;
    private readonly ILogger<DiscordBotCleanupRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
    private readonly IDiscordConnectionHandler _connectionHandler;

    public DiscordBotCleanupRepository(AuthDbContext db, ILogger<DiscordBotCleanupRepository> logger, IConfiguration configuration, IUnitOfWork<AuthDbContext> unitOfWork, IDiscordConnectionHandler connectionHandler)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _connectionHandler = connectionHandler;
    }

    async Task IDiscordBotCleanupRepository.CleanUp()
    {
        try
        {
            var client = await _connectionHandler.GetDiscordSocketRestClient(_configuration["Discord:Token"] ?? string.Empty);


            var expiredLicenses = await _db.ActiveLicenses
                .Include(user => user.User)
                .Where(time => DateTime.UtcNow >= time.EndDate).ToListAsync();

            var allActiveLicenses = await _db.ActiveLicenses.Include(user => user.User)
                .Where(time => DateTime.UtcNow < time.EndDate).ToListAsync();

            if (expiredLicenses.Any())
            {
                _db.RemoveRange(expiredLicenses);
                await _db.SaveChangesAsync();
            }

            try
            {
                var guild = client.socketClient.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));
                var guildMembers = await guild.GetUsersAsync().FlattenAsync();


                foreach (var member in guildMembers.Where(x => x.RoleIds.ToList().Count > 1))
                {
                    IGuildUser? guildUser = null;

                    if (guild != null)
                    {
                        var clientUser = await client.socketClient.GetUserAsync(member.Id);
                        guildUser = guild.GetUser(clientUser.Id);
                    }

                    if (guildUser != null && (expiredLicenses.Exists(x => x.User.DiscordId == member.Id.ToString())
                        || !allActiveLicenses.Exists(user => user.User.DiscordId == member.Id.ToString())))
                    {

                        _ = ulong.TryParse(_configuration["Discord:Role:Admin"], out ulong adminRoleId);
                        _ = ulong.TryParse(_configuration["Discord:Role:Staff"], out ulong staffRoleId);
                        _ = ulong.TryParse(_configuration["Discord:Role:Booster"], out ulong ServerBoosterRoleId);
                        _ = ulong.TryParse(_configuration["Discord:Role:CommunityHero"], out ulong communityHeroRoleId);

                        var rolesNotToRemove = new HashSet<ulong>
                        {
                            860603152280584222, //Everyone
                            adminRoleId,
                            staffRoleId,
                            ServerBoosterRoleId,
                            communityHeroRoleId,
                        };

                        var roles = guildUser.RoleIds.Where(id => !rolesNotToRemove.Contains(id)).ToList();

                        try
                        {
                            await guildUser.RemoveRolesAsync(roles);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("Failed to remove role: " + ex.Message + $"\n{guildUser.Username + "#" + guildUser.Discriminator}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Role Removal Exception: " + ex.Message);
            }


            _logger.LogInformation("NotificationReady Roles & DB Purge");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Unexpected error: " + ex.Message);
        }
    }
}