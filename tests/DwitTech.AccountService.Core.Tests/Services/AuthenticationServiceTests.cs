using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.repository;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class AuthenticationServiceTests
    {
        //Arrange
        private User _user;
        private IConfiguration _configuration;
        private Mock<ISecurityService> _mockSecurityService;
        private Mock<IAuthenticationRepository> _mockAuthRepository;
        private IAuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _user = new User
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

            //generate 128 bit key
            byte[] key = new byte[16];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
            }
            string base64Key = Convert.ToBase64String(key);

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", base64Key},
                    {"Jwt:Issuer", "testIssuer"},
                    {"JwtAuduence", "testAudience"},
                    {"Jwt:JwtTokenExpiryTime", "15"}
                })
                .Build();


            _mockSecurityService = new Mock<ISecurityService>();
            _mockSecurityService.Setup(x => x.HashString(It.IsAny<string>())).Returns("hashed_string");

            _mockAuthRepository = new Mock<IAuthenticationRepository>();

            _authService = new AuthenticationService(_configuration, _mockAuthRepository.Object, _mockSecurityService.Object);
        }


        [Fact]
        public async Task GenerateAccessToken_Returns_Valid_TokenModel_When_User_Is_Valid()
        {
            //Act
            var result = await _authService.GenerateAccessToken(_user);

            //Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.accessToken));
            Assert.False(string.IsNullOrEmpty(result.refreshToken));
            Assert.Equal(60*15, result.expiresIn);
            Assert.Equal("Bearer", result.tokenType);
            _mockSecurityService.Verify((x => x.HashString(It.IsAny<string>())), Times.Once());
        }


        [Fact]
        public void GenerateRefreshToken_Returns_Valid_String_Result()
        {
            //Act
            var result = _authService.GenerateRefreshToken();
            
            //Assert
            Assert.NotEmpty(result);
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }


        public async Task GenerateAccessTokenFromRefreshToken_Returns_Valid_TokenModel()
        {
        }
    }
}
