using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
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

        [Fact(Skip = "todo")]
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

        [Fact(Skip = "todo")]
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
        public async Task CreateUser_Should_Return_CreeatedResult_If_TaskCompletes_Successfully()
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
            Assert.True(result is CreatedResult);
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
        public async Task ChangePassword_ShouldReturnBadRequest_If_ChangePasswordAsyncThrowsException()
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

        [Fact]
        public async Task Logout_Should_Return_True()
        {
            var userService = new Mock<IUserService>();
            var authService = new Mock<IAuthenticationService>();
            var actService = new Mock<IActivationService>();
            var iloggerMock = new Mock<ILogger<UserController>>();
            string authHeader = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoidXNlckBleGFtcGxlLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6IkphbWVzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc3VybmFtZSI6IkpvaG4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNjgxNDk3NTA2LCJpc3MiOiJ0ZXN0SXNzdWVyIn0.uaiS1np_dt7-DPr2ot5JLf_fffdeT0iza83al8n7jNM";

            userService.Setup(x => x.LogoutUser(authHeader)).Returns(Task.FromResult(true));
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Method = "DELETE";
            request.Headers["Authorization"] = authHeader;
            var controller = new UserController(actService.Object, authService.Object, userService.Object, iloggerMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            var result = await controller.Logout();
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task Logout_Should_Return_False_If_AuthorizationHeader_Is_Not_Given()
        {
            var userService = new Mock<IUserService>();
            var authService = new Mock<IAuthenticationService>();
            var actService = new Mock<IActivationService>();
            var iloggerMock = new Mock<ILogger<UserController>>();
            string authHeader = "Bearer ";
            userService.Setup(x => x.LogoutUser(authHeader)).Returns(Task.FromResult(true));
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Method = "DELETE";
            var controller = new UserController(actService.Object, authService.Object, userService.Object, iloggerMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            var result = await controller.Logout();
            userService.Verify(x => x.LogoutUser(authHeader), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ResetPassword_Should_ReturnOkResult_When_RequestIsValid()
        {
            // Arrange
            var email = new UserEmailRequestModel { UserEmail = "testuser@test.com" };
            var authService = new Mock<IAuthenticationService>();
            var actService = new Mock<IActivationService>();
            var iloggerMock = new Mock<ILogger<UserController>>();
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.ResetPassword(email.UserEmail)).ReturnsAsync(true);

            var controller = new UserController(actService.Object, authService.Object, userService.Object, iloggerMock.Object);

            // Act
            var result = await controller.ResetPassword(email);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task ResetPassword_Should_ReturnBadRequestResult_When_RequestIsInvalid()
        {
            // Arrange
            var email = new UserEmailRequestModel { UserEmail = "invalidemail" };
            var authService = new Mock<IAuthenticationService>();
            var actService = new Mock<IActivationService>();
            var iloggerMock = new Mock<ILogger<UserController>>();
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.ResetPassword(email.UserEmail)).Throws(new ArgumentException());

            var controller = new UserController(actService.Object, authService.Object, userService.Object, iloggerMock.Object);

            // Act
            var result = await controller.ResetPassword(email);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ResetPassword_Should_ReturnBadRequestResult_When_RestPasswordInServiceThrowsException()
        {
            // Arrange
            var email = new UserEmailRequestModel { UserEmail = "testuser@test.com" };
            var authService = new Mock<IAuthenticationService>();
            var actService = new Mock<IActivationService>();
            var iloggerMock = new Mock<ILogger<UserController>>();
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.ResetPassword(email.UserEmail)).Throws(new Exception());

            var controller = new UserController(actService.Object, authService.Object, userService.Object, iloggerMock.Object);

            // Act
            var result = await controller.ResetPassword(email);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}