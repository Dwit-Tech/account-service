using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Middleware;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DwitTech.AccountService.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly IConfiguration _configuration;

        public UserControllerTest()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FROM_EMAIL","example@gmail.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"}

            }).Build();
        }
      
        [Fact]
        public void ActivateUser_ShouldReturn_HTTP200()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var iHttpClientFactory = new Mock<IHttpClientFactory>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var _mockAuthService = new AuthenticationService(_configuration, authRepository.Object);
            var userController = new UserController(_mockService, _mockAuthService);
            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
      
        [Fact]
        public void AuthenticateUserLogin_ShouldReturn_Ok()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var iHttpClientFactory = new Mock<IHttpClientFactory>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockService = new Mock<ActivationService>(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var _mockAuthService = new AuthenticationService(_configuration, authRepository.Object);

            var userController = new UserController(_mockService.Object, _mockAuthService);

            string email = "hello@support.com";
            string hashedPassword = "whgwygy37t63t36shhcxvw";

            //act
            var actual = userController.AuthenticateUserLogin(email, hashedPassword);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}