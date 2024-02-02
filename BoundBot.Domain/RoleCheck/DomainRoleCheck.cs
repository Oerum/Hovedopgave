using BoundBot.Components.RestModel;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BoundBot.Domain.RoleCheck;

public class DomainRoleCheck : IDomainRoleCheck
{
    private readonly ILogger<DomainRoleCheck> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDiscordConnectionHandler _discordConnectionHandler;

    public DomainRoleCheck(ILogger<DomainRoleCheck> logger, IConfiguration configuration, IDiscordConnectionHandler discordConnectionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordConnectionHandler = discordConnectionHandler;
    }

    async Task<bool> IDomainRoleCheck.RoleCheck(SocketSlashCommand command, RoleCheckEnum role)
    {
        try
        {
            DiscordModelDtoRestModel restModel = new(command);
            var embedBuilder = new EmbedBuilder
            {
                Color = Color.DarkOrange,
                Timestamp = DateTimeOffset.Now
            };

            bool hasAccess = false;

            switch (role)
            {
                case RoleCheckEnum.BoundMod:
                    hasAccess = restModel.Model.Roles!.Contains(_configuration["Discord:Role:Admin"]!);
                    break;

                case RoleCheckEnum.BoundStaffAndAbove:
                    hasAccess = restModel.Model.Roles!.Contains(_configuration["Discord:Role:Admin"]!) ||
                                restModel.Model.Roles.Contains(_configuration["Discord:Role:Staff"]!);
                    break;

                case RoleCheckEnum.BoundServerBooster:
                    hasAccess = restModel.Model.Roles!.Contains(_configuration["Discord:Role:Boost"]!);
                    break;

                case RoleCheckEnum.BoundAll:
                    hasAccess = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }

            if (hasAccess)
            {
                return true;
            }

            embedBuilder.AddField("Denied", $"You do not have access to {command.CommandName}");

            try
            {
                await command.RespondAsync(embed: embedBuilder.Build());
            }
            catch (Exception)
            {
                DiscordSocketClient discordClient =
                    _discordConnectionHandler.GetDiscordSocketClient(_configuration["Discord:Token"] ?? string.Empty);

                var privateChannel = await discordClient.GetChannelAsync(command.Channel.Id);
                var textNotifier = privateChannel as IMessageChannel;
                await textNotifier!.SendMessageAsync(embed: embedBuilder.Build());
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(1, ex, "Discord Role Domain Failure");
        }

        return false;
    }

}