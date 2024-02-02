using System.Text;
using Auth.Database;
using BoundBot.Connection.DiscordConnectionHandler;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Database.Application.Interface;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using ConnectionState = System.Data.ConnectionState;

namespace Database.Infrastructure;

public class MariaDbBackupRepository : IMariaDbBackupRepository
{
    private readonly AuthDbContext _authDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MariaDbBackupRepository> _logger;
    private readonly IDiscordConnectionHandler _connectionHandler;

    public MariaDbBackupRepository(AuthDbContext authDbContext, IConfiguration configuration, ILogger<MariaDbBackupRepository> logger, IDiscordConnectionHandler connectionHandler)
    {
        _authDbContext = authDbContext;
        _configuration = configuration;
        _logger = logger;
        _connectionHandler = connectionHandler;
    }

    async Task IMariaDbBackupRepository.Backup()
    {
        try
        {
            _logger.LogInformation("Database Backup Logic Start");

            var connectionString = _configuration["ConnectionStrings:DB:BC"];

            await using var conn = new MySqlConnection(connectionString);

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            await using var cmd = new MySqlCommand();
            cmd.Connection = conn;

            using var backup = new MySqlBackup(cmd);

            var fileName = $"MariaDbBackup_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}.sql";

            var backupString = backup.ExportToString();

            var backupStream = new MemoryStream(Encoding.UTF8.GetBytes(backupString));

            await conn.CloseAsync();

            DiscordSocketClient client =
                _connectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

            var guild = client.GetGuild(ulong.Parse(_configuration["Discord:Guid"]!));
            
            var privateChannel = await client.GetChannelAsync(1094738809121423380); // backupChannel

            if (privateChannel is IMessageChannel channel)
            {
                var allMessages = await channel.GetMessagesAsync().FlattenAsync();

                foreach (var message in allMessages.Where(x => x.Timestamp >= x.Timestamp.AddDays(30)))
                {
                    await message.DeleteAsync();
                }

                var roles = guild.Roles;

                var role = roles.FirstOrDefault(x=>x.Name == "Backup" || x.Id == 1095419684892975325);

                if (role != null)
                {
                    var result = await channel.SendFileAsync(backupStream, fileName, $"{role.Mention}\n:white_small_square:Backup_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}", false, null);

                    _logger.LogInformation("Database Backup Logic Finished");
                }
                else
                {
                    _logger.LogError("Backup stream or role is null");
                    throw new Exception("Backup stream or role is null");
                }
            }
            else
            {
                _logger.LogError("!privateChannel is IMessageChannel channel");
                throw new Exception("!privateChannel is IMessageChannel channel");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup Service Error");
            throw new Exception("Database Backup Error: " + ex.Message);
        }
    }
}