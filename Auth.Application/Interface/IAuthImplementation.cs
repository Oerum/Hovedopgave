using Auth.Components;
using Auth.Database.Model;
using Crosscutting;

namespace Auth.Application.Interface
{
    public interface IAuthImplementation
    {
        Task<List<AuthModelDTO>> Auth(AuthModelDTO model);
        Task<DiscordOAuthModel> DiscordOAuth(DiscordOAuthDTO model);
    }
}
