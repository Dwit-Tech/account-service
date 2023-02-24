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
using DwitTech.AccountService.Data.repository;

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


        public async Task<TokenModel> GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Firstname),
                new Claim(ClaimTypes.Surname, user.Lastname),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:JwtTokenExpiryTime"])),
                signingCredentials: credentials);
            
            var AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var RefreshToken = GenerateRefreshToken();

            var currentUserSession = await GetSessionByUserIdAsync(user.Id);

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
                    UserId = user.Id,
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<TokenModel> GenerateAccessTokenfromRefreshToken(TokenModel tokenModel)
        {
            // This method generates new access and refresh tokens, and updates the refresh token in the db.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Get principal from expired access token
            var principal = GetPrincipalFromExpiredToken(tokenModel.accessToken);

            // Create a new JWT token
            var newClaims = new[]
            {
                new Claim("UserId", principal.FindFirst("UserId").Value),
                new Claim(ClaimTypes.Email, principal.FindFirst(ClaimTypes.Email).Value),
                new Claim(ClaimTypes.GivenName, principal.FindFirst(ClaimTypes.GivenName).Value),
                new Claim(ClaimTypes.Surname, principal.FindFirst(ClaimTypes.Surname).Value),
                new Claim(ClaimTypes.Role, "User")
            };
                        
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                newClaims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:JwtTokenExpiryTime"])),
                signingCredentials: credentials);

            // Serialize the token to a string
            string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            //store tokens
            var newAccessToken = AccessToken;
            var newRefreshToken = GenerateRefreshToken();

            //update refresh token to db in hashed format
            var currentSession = await GetSessionByUserIdAsync(int.Parse(principal.FindFirst("UserId").Value));
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
