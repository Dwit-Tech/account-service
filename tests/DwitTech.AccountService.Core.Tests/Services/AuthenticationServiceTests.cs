using DwitTech.AccountService.Core.Exceptions;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Security.Claims;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class AuthenticationServiceTests
    {
        //Setup
        private User mockUser;
        private IConfiguration _configuration;
        private Mock<IAuthenticationRepository> mockAuthRepository;
        private IAuthenticationService authService;
        private const string tokenType = "Bearer";
        private const string validJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIxIiwi" +
                "aHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiZmlyc3RuYW1lQGdtYWlsLmNvbSIsImh0" +
                "dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6IkZpcnN0TmFtZSIsImh0dHA6Ly9zY2hlbWFzLnhtb" +
                "HNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3N1cm5hbWUiOiJMYXN0TmFtZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8" +
                "wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE2Nzc3ODczMzQsImlzcyI6InRlc3RJc3N1ZXIifQ.PMOYitdiHpshT04kaUb0yJUWlnjtMFjVh_3kpblEsN8";
        private const string validRefreshToken = "HC2MBsmq7zRY3iWc3jvkjukYu/jdYab9c5+MENq2+RQ=";
        private List<Claim> claims;

        public AuthenticationServiceTests()
        {
            mockUser = new User
            {
                Id = 1,
                Firstname = "FirstName",
                Lastname = "LastName",
                Email = "firstname@gmail.com",
                PhoneNumber = "phonenumber",
                AddressLine1 = "addressLine1",
                AddressLine2 = "AddressLine2",
                Country = "country",
                State = "state",
                City = "city",
                PostalCode = "postalCode",
                ZipCode = "zipcode",
                Password = "password",
                ModifiedOnUtc = DateTime.UtcNow
            };

            claims = new List<Claim>
            {
                new Claim("UserId", mockUser.Id.ToString()),
                new Claim(ClaimTypes.Email, mockUser.Email),
                new Claim(ClaimTypes.GivenName, mockUser.Firstname),
                new Claim(ClaimTypes.Surname, mockUser.Lastname),
                new Claim(ClaimTypes.Role, "User")
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "oAZjokG5AONGvEX80R/ggQ=="},
                    {"Jwt:Issuer", "testIssuer"},
                    {"Jwt:JwtTokenExpiryTime", "15"},
                    {"Jwt:RefreshTokenExpiryTime", "1"}
                })
                .Build();

            mockAuthRepository = new Mock<IAuthenticationRepository>();

            authService = new AuthenticationService(_configuration, mockAuthRepository.Object);           
        }
 

        [Fact]
        public async Task GenerateAccessToken_Returns_TokenModel_When_User_Is_Valid()
        {
            //Act
            var result = await authService.GenerateAccessToken(mockUser);

            //Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
            Assert.Equal(60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]), result.ExpiresIn);
            Assert.Equal(tokenType, result.TokenType);            
        }


        [Fact]
        public async Task GenerateSecurityTokens_CallsMethodToAddNewSession_When_SessionDoesNotExist()
        {
            //Arrange
            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>())).ReturnsAsync((SessionToken?)null);

            //Act
            var result = await authService.GenerateAccessToken(mockUser);

            //Assert
            mockAuthRepository.Verify(x => x.FindSessionByUserIdAsync(mockUser.Id), Times.Once);
            mockAuthRepository.Verify(x => x.AddSessionAsync(It.IsAny<SessionToken>()), Times.Once);
            mockAuthRepository.Verify(x => x.UpdateSessionTokenAsync(), Times.Never);
        }


        [Fact]
        public async Task GenerateSecurityTokens_CallsMethodToUpdateSession_When_SessionExists()
        {
            //Arrange
            var sessionToken = new SessionToken
            {
                UserId = mockUser.Id,
                RefreshToken = StringUtil.HashString(validRefreshToken),
                ModifiedOnUtc = DateTime.UtcNow - TimeSpan.FromHours(int.Parse(_configuration["Jwt:RefreshTokenExpiryTime"]))
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>())).ReturnsAsync((sessionToken));

            //Act
            var result = await authService.GenerateAccessToken(mockUser);

            //Assert
            mockAuthRepository.Verify(x => x.FindSessionByUserIdAsync(mockUser.Id), Times.Once);
            mockAuthRepository.Verify(x => x.UpdateSessionTokenAsync(), Times.Once);
            mockAuthRepository.Verify(x => x.AddSessionAsync(It.IsAny<SessionToken>()), Times.Never);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ReturnsTokenModel_WhenModifiedOnUtcValueIsPresent_AndTokenIsValid()
        {
            // Arrange
            var accessToken = JwtUtil.GenerateJwtToken(claims, _configuration);
            var refreshToken = StringUtil.GenerateRandomBase64string();

            var sessionToken = new SessionToken
            {
                UserId = mockUser.Id,
                RefreshToken = StringUtil.HashString(refreshToken),
                ModifiedOnUtc = DateTime.UtcNow
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>())).ReturnsAsync(sessionToken);

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = refreshToken
            };

            // Act
            var result = await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenType, result.TokenType);
            Assert.Equal(60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]), result.ExpiresIn);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ReturnsTokenModel_WhenModifiedOnUtcValueIsNotPresent_AndTokenIsValid()
        {
            // Arrange
            var accessToken = JwtUtil.GenerateJwtToken(claims, _configuration);
            var refreshToken = StringUtil.GenerateRandomBase64string();

            var sessionToken = new SessionToken
            {
                UserId = mockUser.Id,
                RefreshToken = StringUtil.HashString(refreshToken),
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>())).ReturnsAsync(sessionToken);

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = refreshToken
            };

            // Act
            var result = await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenType, result.TokenType);
            Assert.Equal(60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]), result.ExpiresIn);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ThrowsEntityNotFoundException_WhenCurrentSessionIsNull()
        {
            // Arrange
            var expectedExceptionMessage = "The specified entity does not exist in the database";
            var tokenModel = new TokenModel
            {
                AccessToken = validJwtToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = validRefreshToken
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync((SessionToken?)null);

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(act);

            Assert.Equal(expectedExceptionMessage, ex.Message);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ThrowsArgumentException_WhenRefreshTokensDoNotMatch()
        {
            // Arrange
            var expectedExceptionMessage = "Refresh token is not valid. (Parameter 'RefreshToken')";
            var tokenModel = new TokenModel
            {
                AccessToken = validJwtToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = "invalid_refresh_token"
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SessionToken
                {
                    UserId = mockUser.Id,
                    RefreshToken = StringUtil.HashString(validRefreshToken),
                    ModifiedOnUtc = DateTime.UtcNow
                });

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(act);

            Assert.Equal(expectedExceptionMessage, ex.Message);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ThrowsArgumentException_WhenRefreshTokenHasExpired()
        {
            // Arrange
            var expectedExceptionMessage = "Refresh token is not valid. (Parameter 'RefreshToken')";
            var tokenModel = new TokenModel
            {
                AccessToken = validJwtToken,
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = validRefreshToken
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SessionToken
                {
                    UserId = mockUser.Id,
                    RefreshToken = StringUtil.HashString(validRefreshToken),
                    ModifiedOnUtc = DateTime.UtcNow-TimeSpan.FromHours(int.Parse(_configuration["Jwt:RefreshTokenExpiryTime"]))
                });

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(act);

            Assert.Equal(expectedExceptionMessage, ex.Message);
        }


        [Fact]
        public async Task GenerateAccessTokenFromRefreshToken_ThrowsSecurityTokenException_WhenUserIdIsMissingFromClaims()
        {
            // Arrange
            var expectedExceptionMessage = "Missing UserId claim.";
            claims = new List<Claim>
            {
                //Id is omitted.
                new Claim(ClaimTypes.Email, mockUser.Email),
                new Claim(ClaimTypes.GivenName, mockUser.Firstname),
                new Claim(ClaimTypes.Surname, mockUser.Lastname),
                new Claim(ClaimTypes.Role, "User")
            };

            var tokenModel = new TokenModel
            {
                AccessToken = JwtUtil.GenerateJwtToken(claims, _configuration),
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = "invalid_refresh_token"
            };

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<SecurityTokenException>(act);
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }


        [Fact]
        public async Task ValidateRefreshToken_ThrowsEntityNotFoundException_When_UserSessionDoesNotExist()
        {
            // Arrange
            var expectedExceptionMessage = "The specified entity does not exist in the database";
           
            var tokenModel = new TokenModel
            {
                AccessToken = JwtUtil.GenerateJwtToken(claims, _configuration),
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = "invalid_refresh_token"
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>())).ReturnsAsync((SessionToken?)null);

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(act);
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }


        [Fact]
        public async Task GetPrincipalFromExpiredToken_ThrowsSecurityTokenException_WhenJwtkeySignatureIsInvalid()
        {
            // Arrange
            var expectedExceptionMessage = "Invalid access token";
            var tokenModel = new TokenModel
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIxIiwi" +
                "aHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZ" +
                "XNzIjoiZmlyc3RuYW1lQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUv" +
                "aWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6IkZpcnN0TmFtZSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL" +
                "3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3N1cm5hbWUiOiJMYXN0T",
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = validRefreshToken
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SessionToken
                {
                    UserId = mockUser.Id,
                    RefreshToken = StringUtil.HashString(validRefreshToken),
                    ModifiedOnUtc = DateTime.UtcNow
                });

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<SecurityTokenException>(act);

            Assert.Equal(expectedExceptionMessage, ex.Message);
        }

        [Fact]
        public async Task GetPrincipalFromExpiredToken_ThrowsSecurityTokenException_WhenAccessTokenIsNotJwt()
        {
            // Arrange
            var expectedExceptionMessage = "Invalid access token";
            var tokenModel = new TokenModel
            {
                AccessToken = "certainly_not_jwt_token",
                TokenType = tokenType,
                ExpiresIn = 60 * int.Parse(_configuration["Jwt:JwtTokenExpiryTime"]),
                RefreshToken = validRefreshToken
            };

            mockAuthRepository.Setup(x => x.FindSessionByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SessionToken
                {
                    UserId = mockUser.Id,
                    RefreshToken = StringUtil.HashString(validRefreshToken),
                    ModifiedOnUtc = DateTime.UtcNow
                });

            // Act
            async Task<TokenModel> act() => await authService.GenerateAccessTokenFromRefreshToken(tokenModel);

            // Assert
            var ex = await Assert.ThrowsAsync<SecurityTokenException>(act);

            Assert.Equal(expectedExceptionMessage, ex.Message);
        }
    }
}
