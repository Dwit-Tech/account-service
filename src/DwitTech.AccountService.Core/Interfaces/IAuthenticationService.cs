using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenModel> GenerateAccessToken(User user);
        Task<TokenModel> GenerateAccessTokenFromRefreshToken(TokenModel tokenModel);
    }
}
