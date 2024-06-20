using Crosscutting;

namespace Auth.Domain
{
    public interface IAuthDomain
    {
        public Task<bool> AuthenticateAsync(List<AuthModelDTO> auths);
    }
    public class AuthDomain : IAuthDomain
    {
        async Task<bool> IAuthDomain.AuthenticateAsync(List<AuthModelDTO> auths)
        {
            if (auths.Count != 0 && auths.Any(x=>x.Success == true))
            {
                return await Task.FromResult(true);
            }
            
            return await Task.FromResult(false);
        }
    }
}