using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            var _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FROM_EMAIL","example@gmail.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"}
            
            }).Build();

            var _activationService = new Mock<IActivationService>();
            var _mockUserService = new Mock<IUserService>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockAuthService = new Mock<AuthenticationService>(_configuration, authRepository.Object);

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }

        [Fact]
        public void AuthenticateUserLogin_ReturnsOk_WhenLoginSuccessful()
        {
             var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var _activationService = new Mock<IActivationService>();
            var authRepository = new Mock<AuthenticationRepository>(mockDbContext.Object);
            var _mockUserService = new Mock<IUserService>(); 
            var _mockAuthService = new Mock<AuthenticationService>(_configuration, authRepository.Object);

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            var requestLoginDto = new LoginRequestDto
            {
                Email = "hello@support.com",
                Password = "whgwygy37t63t36shhcxvw"
            };

            //act
            var actual = userController.AuthenticateUserLogin(requestLoginDto);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task AuthenticateUserLogin_ReturnsBadRequestResult_WhenLoginUnsuccessful()
        {
            // Arrange
            var requestLoginDto = new LoginRequestDto
            {
                Email = "hello@support.com",
                Password = "incorrectpassword"
            };

            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();
            var _mockUserService = new Mock<IUserService>();

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            // Act
            var result = await userController.AuthenticateUserLogin(requestLoginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
        #nullable enable
            Assert.Equal("Invalid email or password.", badRequestResult?.Value);
        #nullable disable
        }

        [Fact]
        public async Task CreateUser_Should_Return_Ok_If_TaskCompletes_Successfully()
        {
            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();

            var _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.CreateUser(It.IsAny<UserDto>())).Returns(Task.FromResult(true));

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

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());
            var result = await userController.CreateUser(userDto);

            _mockUserService.Verify(x => x.CreateUser(It.IsAny<UserDto>()), Times.Once);
            Assert.True(result is OkObjectResult);
        }

        [Fact]
        public async Task CreateUser_Should_Return_BadResult_If_UserFailsToCreate()
        {
            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();

            var _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.CreateUser(It.IsAny<UserDto>())).Returns(Task.FromResult(false));

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

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());
            var result = await userController.CreateUser(userDto);

            _mockUserService.Verify(x => x.CreateUser(It.IsAny<UserDto>()), Times.Once);
            Assert.True(result is BadRequestObjectResult);
        }


        [Fact]
        public async Task CreateUser_Should_Throw_Exception_If_CreateUserThrows()
        {
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

            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();
            var _mockLogger = new Mock<ILogger<UserController>>();
            var _mockUserService = new Mock<IUserService>();

            _mockUserService.Setup(x => x.CreateUser(userDto)).Throws(new Exception("Test exception"));

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, _mockLogger.Object);

            // Act
            var result = await userController.CreateUser(userDto);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}