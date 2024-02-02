using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BoundBot.Components.Members
{
    public interface IDiscordServerMembersHandler
    {
        public Task Initialize(DiscordSocketClient client);
        public Task Add(SocketGuildUser user);
        public Task Remove(SocketGuild guild, SocketUser user);
        public Task<IGuildUser?> GetUser(ulong discordId);

    }

    public static class MembersData
    {
        public static List<IGuildUser>? AllGuildMembers { get; set; } = new();
    }

    public class DiscordServerMembersHandler : IDiscordServerMembersHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IDiscordConnectionHandler _connectionHandler;
        private readonly ILogger<DiscordServerMembersHandler> _logger;

        public DiscordServerMembersHandler(IDiscordConnectionHandler connectionHandler, IConfiguration configuration, ILogger<DiscordServerMembersHandler> logger)
        {
            _connectionHandler = connectionHandler;
            _configuration = configuration;
            _logger = logger;
        }

        async Task IDiscordServerMembersHandler.Initialize(DiscordSocketClient client)
        {
            try
            {
                var guildId = ulong.Parse(_configuration["Discord:Guid"]!);

                // Get the guild by ID
                var guild = client.GetGuild(guildId);

                if (guild != null)
                {
                    List<IGuildUser> allGuildMembers;

                    if (MembersData.AllGuildMembers!.Count <= 0)
                    {
                        // Retrieve guild members outside of the lock
                        allGuildMembers = (List<IGuildUser>)await guild.GetUsersAsync().FlattenAsync();

                        // Lock to ensure safe access to the list when updating
                        lock (MembersData.AllGuildMembers)
                        {
                            // Check if another thread has already populated the list
                            if (MembersData.AllGuildMembers.Count <= 0)
                            {
                                // Populate the member list within the lock
                                MembersData.AllGuildMembers.AddRange(allGuildMembers);
                            }
                            else
                            {
                                // If the list is already populated, simply copy its contents
                                allGuildMembers = new List<IGuildUser>(MembersData.AllGuildMembers);
                            }
                        }
                    }
                    else
                    {
                        // If the list is already populated, simply copy its contents
                        allGuildMembers = new List<IGuildUser>(MembersData.AllGuildMembers);
                    }

                    // Log member count
                    _logger.LogInformation($"Discord Member Count: {allGuildMembers.Count}");
                }
                else
                {
                    _logger.LogWarning("Guild not found with the specified ID.");
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid guild ID format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during members list initialization.");
            }
        }

        async Task<IGuildUser?> IDiscordServerMembersHandler.GetUser(ulong discordId)
        {
            IGuildUser? user;

            lock (MembersData.AllGuildMembers!)
            {
                user = MembersData.AllGuildMembers.FirstOrDefault(x => x.Id == discordId);
            }

            return await Task.FromResult(user);
        }

        Task IDiscordServerMembersHandler.Add(SocketGuildUser user)
        {
            try
            {
                lock (MembersData.AllGuildMembers!)
                {
                    if (!MembersData.AllGuildMembers.Contains(user))
                    {
                        MembersData.AllGuildMembers.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1, ex, "Member Add Error");
            }

            return Task.CompletedTask;
        }

        public Task Remove(SocketGuild guild, SocketUser user)
        {
            try
            {
                lock (MembersData.AllGuildMembers!)
                {
                    // Use RemoveAll to efficiently remove all matching users from the list
                    MembersData.AllGuildMembers.RemoveAll(guildUser => guildUser.Id == user.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Member Remove Error");
            }

            return Task.CompletedTask;
        }

    }


}
