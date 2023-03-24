using DwitTech.AccountService.Core.Utilities;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DwitTech.AccountService.Core.Tests.Utilities
{
    public class JwtUtilTests
    {
        //Arrange
        private IConfiguration _configuration;
        private List<Claim> claims;

        public JwtUtilTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"JWT_KEY", "oAZjokG5AONGvEX80R/ggQ=="},
                    {"JWT_ISSUER", "testIssuer"},
                    {"JWT_TOKEN_EXPIRY_MINUTES", "15"}
                })
                .Build();

            claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "Name"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "testuser@example.com")
            };
        }

        [Fact]
        public void GenerateJwtToken_ReturnsValidToken_WhenClaimsProvided()
        {
            // Act
            var token = JwtUtil.GenerateJwtToken(claims, _configuration);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void GenerateJwtToken_ReturnsTokenWithExpectedValues_WhenClaimsAndConfigurationProvided()
        {
            // Act
            var token = JwtUtil.GenerateJwtToken(claims, _configuration);

            var jwtHandler = new JwtSecurityTokenHandler();
            var decodedToken = jwtHandler.ReadJwtToken(token);

            // Assert
            Assert.Equal(_configuration["JWT_ISSUER"], decodedToken.Payload["iss"]);
            foreach (var claim in claims)
            {
                Assert.Contains(decodedToken.Claims, c => c.Type == claim.Type && c.Value == claim.Value);
            }

            Assert.True(decodedToken.ValidTo > DateTime.UtcNow);
            Assert.True(decodedToken.ValidFrom < DateTime.UtcNow);
        }
    }
}
