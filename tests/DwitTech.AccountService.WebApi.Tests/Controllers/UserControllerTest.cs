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
        private readonly Mock<IActivationService> _mockActService;
        private readonly UserController _controller;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<string> _apiKey;
        private readonly Mock<string[]> _allowedIpAddresses;
      
        [Fact]
        public void ActivateUser_ShouldReturn_HTTP200()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FROM_EMAIL","example@gmail.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"}

            }).Build();

            var iHttpClientFactory = new Mock<IHttpClientFactory>();
            var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var userController = new UserController(_mockService);
            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
      
        [Fact]
        public void UserLogin_ShouldReturn_Ok()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var iConfig = new Mock<IConfiguration>();
            var authenticationService = new Mock<IAuthenticationService>();
            var mockNext = new Mock<RequestDelegate>();
            var authorizationMiddleware = new Mock<AuthorizationMiddleware>(mockNext.Object, _apiKey, _allowedIpAddresses);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var _mockService = new Mock<ActivationService>(iConfig.Object, userRepository.Object);
            var _userService = new Mock<UserService>(userRepository.Object, iConfig.Object, authenticationService.Object, authorizationMiddleware.Object, httpContextAccessor.Object);

            var userController = new UserController(_mockService.Object, _userService.Object);

            string email = "hello@support.com";
            string hashedPassword = "whgwygy37t63t36shhcxvw";

            //act
            var actual = userController.UserLogin(email, hashedPassword);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}