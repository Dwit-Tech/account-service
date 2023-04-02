using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.Core.Utilities;

namespace DwitTech.AccountService.Core.Services
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _repository;
        private const string tokenType = "Bearer";

        public AuthenticationService(IConfiguration configuration, IAuthenticationRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }


        public async Task<TokenModel> GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "User")
            };

            return await GenerateSecurityTokens(user.Id, claims);
        }


        private async Task<TokenModel> GenerateSecurityTokens(int userId, List<Claim> claims)
        {
            var accessToken = JwtUtil.GenerateJwtToken(claims, _configuration);
            var refreshToken = Guid.NewGuid().ToString();

            var currentUserSession = await _repository.FindSessionByUserIdAsync(userId);

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["JWT_TOKEN_EXPIRY_MINUTES"]), //convert to seconds
                RefreshToken = refreshToken
            };

            if (currentUserSession != null)
            {
                currentUserSession.RefreshToken = StringUtil.HashString(refreshToken);
                currentUserSession.ModifiedOnUtc = DateTime.UtcNow;
                await _repository.UpdateSessionTokenAsync(currentUserSession);
                return tokenModel;
            }

            var newUserSession = new SessionToken
            {
                UserId = userId,
                RefreshToken = StringUtil.HashString(refreshToken),
            };

            await _repository.AddSessionAsync(newUserSession);

            return tokenModel;
        }
                

        public async Task<TokenModel> GenerateAccessTokenFromRefreshToken(TokenModel tokenModel)
        {
            // This method generates new access and refresh tokens, and updates the refresh token in the db.

            //Get principal from expired access token
            var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);

            var userId = int.Parse(principal!.FindFirst("UserId")?.Value ?? throw new SecurityTokenException("Missing UserId claim."));

            //Validate the refresh token
            var isValid = await ValidateUserRefreshToken(userId, tokenModel.RefreshToken);
            if (!isValid)
            {
                throw new ArgumentException("The refresh token is not valid.");
            }

            var claimsList = new List<Claim>();

            // Add each claim from the principal to the list
            foreach (Claim claim in principal.Claims) 
            {
                claimsList.Add(claim);
            }

            //store tokens
            var newAccessToken = JwtUtil.GenerateJwtToken(claimsList,_configuration);
            var newRefreshToken = Guid.NewGuid().ToString();

            //update refresh token to db in hashed format
            var currentSession = await _repository.FindSessionByUserIdAsync(userId);
            currentSession!.RefreshToken = StringUtil.HashString(newRefreshToken);
            currentSession.ModifiedOnUtc = DateTime.UtcNow;
            await _repository.UpdateSessionTokenAsync(currentSession);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["JWT_TOKEN_EXPIRY_MINUTES"]), //convert to seconds
                RefreshToken = newRefreshToken
            };
        }


        private async Task<bool> ValidateUserRefreshToken(int userId, string refreshToken)
        {
            var currentSession = await _repository.FindSessionByUserIdAsync(userId);
            var hashedrefreshToken = StringUtil.HashString(refreshToken);

            if (currentSession == null)
            {
                return false;
            }

            if (currentSession.ModifiedOnUtc.HasValue)
            {
                return hashedrefreshToken == currentSession.RefreshToken &&
                       DateTime.UtcNow < currentSession.ModifiedOnUtc.Value.AddHours(int.Parse(_configuration["JWT_REFRESH_TOKEN_EXPIRY_HOURS"]));
            }

            return hashedrefreshToken == currentSession.RefreshToken &&
                   DateTime.UtcNow < currentSession.CreatedOnUtc.AddHours(int.Parse(_configuration["JWT_REFRESH_TOKEN_EXPIRY_HOURS"]));
        }


        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new ArgumentException("token should not be null or empty");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                return principal;
            }
            catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException)
            {
                throw new SecurityTokenException("Invalid access token", ex);
            }            
        }
    }
}
