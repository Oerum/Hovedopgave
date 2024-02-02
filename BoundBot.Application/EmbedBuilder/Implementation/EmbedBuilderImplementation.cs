using BoundBot.Application.EmbedBuilder.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.EmbedBuilder.Implementation;

public class EmbedBuilderImplementation : IEmbedBuilderImplementation
{
    private readonly IEmbedBuilderRepository _repository;
    private readonly ILogger <EmbedBuilderImplementation> _logger;
    private readonly IDomainRoleCheck _domainRoleCheck;
    public EmbedBuilderImplementation(IEmbedBuilderRepository repository, ILogger<EmbedBuilderImplementation> logger, IDomainRoleCheck domainRoleCheck)
    {
        _repository = repository;
        _logger = logger;
        _domainRoleCheck = domainRoleCheck;
    }

    async Task IEmbedBuilderImplementation.EmbedBuilder(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var check = await _domainRoleCheck.RoleCheck(command, RoleCheckEnum.BoundStaffAndAbove);

            if (check)
            {
                await _repository.EmbedBuilder(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}