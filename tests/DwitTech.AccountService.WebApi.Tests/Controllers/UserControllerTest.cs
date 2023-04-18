using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
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

        [Fact(Skip = "todo")]
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
            var userController = new UserController(_mockService, _mockAuthService.Object, userService.Object, Mock.Of<ILogger<UserController>>());

            
            //var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object);
            
            //var userController = new UserController(_mockService, _mockAuthService);
            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }

        [Fact(Skip = "todo")]
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

            var userController = new UserController(_mockService.Object, _mockAuthService.Object,_mockUserService.Object, Mock.Of<ILogger<UserController>>());

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


        [Fact(Skip = "todo")]
        public async Task CreateUser_Should_Throw_Exception_If_CreateUserThrows()
        {
            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();

            var _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.CreateUser(It.IsAny<UserDto>())).Throws(new DbUpdateException());

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
            var result = ()=> userController.CreateUser(userDto);

            _mockUserService.Verify(x => x.CreateUser(It.IsAny<UserDto>()), Times.Once);
            await Assert.ThrowsAsync<Exception>(result);
        }


        [Fact]
        public async Task ChangePassword_Should_Return_Ok_If_TaskCompletes_Successfully()
        {
            //Arrange
            var passwordDetails = new ChangePasswordModel()
            {
                CurrentPassword = "currentpassword",
                NewPassword = "newpassword"
            };

            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();
            var _mockUserService = new Mock<IUserService>();

            _mockUserService.Setup(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.FromResult(true));

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            // Act
            var result = await userController.ChangePassword(passwordDetails);

            // Assert
            _mockUserService.Verify(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.True(result is OkObjectResult);
        }


        [Fact]
        public async Task ChangePassword_Should_Return_BadRequest_If_PasswordFailsToChange()
        {
            //Arrange
            var passwordDetails = new ChangePasswordModel()
            {
                CurrentPassword = "currentpassword",
                NewPassword = "newpassword"
            };

            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();
            var _mockUserService = new Mock<IUserService>();

            _mockUserService.Setup(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(Task.FromResult(false));

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            // Act
            var result = await userController.ChangePassword(passwordDetails);

            // Assert
            _mockUserService.Verify(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.True(result is BadRequestObjectResult);
        }


        [Fact]
        public async Task ChangePassword_Should_Throw_Exception_If_ChangePasswordAsyncThrowsException()
        {
            //Arrange
            var passwordDetails = new ChangePasswordModel()
            {
                CurrentPassword = "currentpassword",
                NewPassword = "newpassword"
            };

            var _activationService = new Mock<IActivationService>();
            var _mockAuthService = new Mock<IAuthenticationService>();

            var _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new ArgumentException());

            var userController = new UserController(_activationService.Object, _mockAuthService.Object, _mockUserService.Object, Mock.Of<ILogger<UserController>>());

            // Act
            var result = await userController.ChangePassword(passwordDetails);

            // Assert
            _mockUserService.Verify(x => x.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.True(result is BadRequestObjectResult);
        }
    }
}