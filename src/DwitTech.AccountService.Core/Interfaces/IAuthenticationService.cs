using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;//Remove after test
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenModel> GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<TokenModel> GenerateAccessTokenfromRefreshToken(TokenModel tokenModel);
        bool ValidateAccessToken(string accessToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);//Remove after test
        Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails);
        Task<SessionToken> GetSessionByUserIdAsync(int userId);
        Task<bool> AddSessionAsync(SessionToken sessionDetails);
    }
}
