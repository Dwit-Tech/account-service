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
        private readonly IConfiguration _configuration;

        public UserControllerTest()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FROM_EMAIL","example@gmail.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"},
                {"X_API_KEY", "your_api_key"},
                {"SOURCE_IP", "127.0.0.1"}

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
            var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var authenticationService = new Mock<IAuthenticationService>();
            var mockNext = new Mock<RequestDelegate>();
            var authorizationMiddleware = new Mock<AuthorizationMiddleware>(mockNext.Object, _configuration);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var _userService = new Mock<UserService>(userRepository.Object, _configuration, authenticationService.Object, authorizationMiddleware.Object, httpContextAccessor.Object);
            var userController = new UserController(_mockService, _userService.Object);
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
            var authenticationService = new Mock<IAuthenticationService>();
            var mockNext = new Mock<RequestDelegate>();
            var authorizationMiddleware = new Mock<AuthorizationMiddleware>(mockNext.Object, _configuration);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var _mockService = new Mock<ActivationService>(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var _userService = new Mock<UserService>(userRepository.Object, _configuration, authenticationService.Object, authorizationMiddleware.Object, httpContextAccessor.Object);

            var userController = new UserController(_mockService.Object, _userService.Object);

            string email = "hello@support.com";
            string hashedPassword = "whgwygy37t63t36shhcxvw";

            //act
            var actual = userController.AuthenticateUserLogin(email, hashedPassword);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}