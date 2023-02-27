using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.Core.Utilities;

namespace DwitTech.AccountService.Core.Services
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _repository;
        private readonly ISecurityService _securityService;
        private const string token_Type = "Bearer";

        public AuthenticationService(IConfiguration configuration, IAuthenticationRepository repository, ISecurityService securityService)
        {
            _configuration = configuration;
            _repository = repository;
            _securityService = securityService;
        }


        public async Task<TokenModel> GenerateAccessToken(int userId, List<Claim> claims)
        {
            var AccessToken = RandomUtil.GetJwt(claims, _configuration);
            var RefreshToken = RandomUtil.GenerateRandomBase64string();

            var currentUserSession = await GetSessionByUserIdAsync(userId);

            if (currentUserSession != null)
            {
                currentUserSession.RefreshToken = _securityService.HashString(RefreshToken);
                currentUserSession.ModifiedOnUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                await UpdateSessionTokenAsync(currentUserSession);
            }
            else
            {
                var newUserSession = new SessionToken
                {
                    UserId = userId,
                    RefreshToken = _securityService.HashString(RefreshToken),
                };

                await AddSessionAsync(newUserSession);
            }

            return new TokenModel
            {
                accessToken = AccessToken,
                tokenType = token_Type,
                expiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]), //convert to seconds
                refreshToken = RefreshToken
            };
        }
                

        public async Task<TokenModel> GenerateAccessTokenfromRefreshToken(TokenModel tokenModel)
        {
            // This method generates new access and refresh tokens, and updates the refresh token in the db.

            //Get principal from expired access token
            var principal = GetPrincipalFromExpiredToken(tokenModel.accessToken);

            var userId = int.Parse(principal.FindFirst("UserId")?.Value ?? throw new ArgumentException("Missing UserId claim.", nameof(tokenModel.accessToken)));
            List<Claim> claimsList = new List<Claim>();

            // Add each claim from the principal to the list
            foreach (Claim claim in principal.Claims) 
            {
                claimsList.Add(claim);
            }

            //store tokens
            var newAccessToken = RandomUtil.GetJwt(claimsList,_configuration);
            var newRefreshToken = RandomUtil.GenerateRandomBase64string();

            //update refresh token to db in hashed format
            var currentSession = await GetSessionByUserIdAsync(userId);
            currentSession.RefreshToken = _securityService.HashString(newRefreshToken);
            await UpdateSessionTokenAsync(currentSession);

            return new TokenModel
            {
                accessToken = newAccessToken,
                tokenType = token_Type,
                expiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]), //convert to seconds
                refreshToken = newRefreshToken
            };
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)//Return to private after test
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }            
        }

        public bool ValidateAccessToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]))
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails)
        {
            return await _repository.UpdateSessionTokenAsync(sessionDetails);
        }

        public Task<SessionToken> GetSessionByUserIdAsync(int userId)
        {
            return _repository.FindSessionByUserIdAsync(userId);
        }

        public Task<bool> AddSessionAsync(SessionToken sessionDetails)
        {
            return _repository.AddSessionAsync(sessionDetails);
        }
    }
}
