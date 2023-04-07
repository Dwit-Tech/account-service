using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Middleware;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http;

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
            var _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FROM_EMAIL","example@gmail.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"}
            
            }).Build();

            var _userRepository = new Mock<IUserRepository>();
            var _roleRepository = new Mock<IRoleRepository>();
            var _logger = new Mock<ILogger<UserService>>();
            var _activationService = new Mock<IActivationService>();
            var _emailService = new Mock<IEmailService>();

            var iHttpClientFactory = new Mock<IHttpClientFactory>();
            var iEmailService = new Mock<IEmailService>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockService = new ActivationService(_configuration,userRepository.Object, iEmailService.Object,iHttpClientFactory.Object);
            var _mockAuthService = new Mock<AuthenticationService>(_configuration, authRepository.Object);
            var userService = new Mock<UserService>(_userRepository.Object, _roleRepository.Object, _logger.Object, _activationService.Object, _emailService.Object);
            var userController = new UserController(_mockService, _mockAuthService.Object, userService.Object);

            
            //var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object);
            
            //var userController = new UserController(_mockService, _mockAuthService);
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
            var _mockUserService = new Mock<IUserService>(); 
            var _mockService = new Mock<ActivationService>(_configuration, userRepository.Object, iHttpClientFactory.Object);
            var _mockAuthService = new Mock<AuthenticationService>(_configuration, authRepository.Object);

            var userController = new UserController(_mockService.Object, _mockAuthService.Object,_mockUserService.Object);

            string email = "hello@support.com";
            string hashedPassword = "whgwygy37t63t36shhcxvw";

            //act
            var actual = userController.AuthenticateUserLogin(email, hashedPassword);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }


        [Fact]
        public async Task CreateUser_Should_Return_True_If_TaskCompletes_Successfully()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var _userRepository = new Mock<IUserRepository>();
            var _roleRepository = new Mock<IRoleRepository>();
            var _logger = new Mock<ILogger<UserService>>();
            var _activationService = new Mock<IActivationService>();
            var _emailService = new Mock<IEmailService>();
            var _mockUserService = new Mock<IUserService>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockAuthService = new Mock<AuthenticationService>(_configuration, authRepository.Object);


            var userService = new Mock<UserService>(_userRepository.Object, _roleRepository.Object, _logger.Object, _activationService.Object, _emailService.Object);

            var userDto = new UserDto
            {
                FirstName = "james",
                LastName = "kim",
                Email = "test@gmail.com",
                AddressLine1 = "add1",
                AddressLine2 = "add2",
                City = "Bida",
                State = "Niger",
                ZipCode = "910001",
                Roles = Core.Enums.Role.User,
                Country = "Nigeria",
                PostalCode = "90001",
                PassWord = "securedpassword",
                PhoneNumber = "1234567890"
            };

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object);
            var result = userController.CreateUser(userDto);
            Assert.True(result.IsCompleted);
        }
    }
}