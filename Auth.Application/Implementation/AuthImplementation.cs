using Auth.Application.Interface;
using Auth.Components;
using Auth.Database.Contexts;
using Auth.Database.Model;
using Crosscutting;
using Crosscutting.TransactionHandling;

namespace Auth.Application.Implementation
{
    public class AuthImplementation : IAuthImplementation
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
        private readonly IUnitOfWork<DiscordOAuthContext> _unitOfWork_oAuth;

        public AuthImplementation(IAuthRepository authRepository, IUnitOfWork<AuthDbContext> unitOfWork, IUnitOfWork<DiscordOAuthContext> unitOfWork_oAuth)
        {
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
            _unitOfWork_oAuth = unitOfWork_oAuth;
        }

        async Task<List<AuthModelDTO>> IAuthImplementation.Auth(AuthModelDTO model)
        {
            try
            {
                await _unitOfWork.CreateTransaction(System.Data.IsolationLevel.RepeatableRead);
                var auth = await _authRepository.Auth(model);  
                await _unitOfWork.Commit();
                return auth;
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }
        }

        async Task<DiscordOAuthModel> IAuthImplementation.DiscordOAuth(DiscordOAuthDTO model)
        {
            try
            {
                await _unitOfWork_oAuth.CreateTransaction(System.Data.IsolationLevel.RepeatableRead);
                var discordOAuth = await _authRepository.DiscordOAuth(model);
                await _unitOfWork_oAuth.Commit();

                return discordOAuth;
            }
            catch
            {
                await _unitOfWork_oAuth.Rollback();
                throw;
            }
        }
    }
}
