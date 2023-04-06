using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Middleware;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Text;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IAuthenticationService> _authenticationService;
        private readonly Mock<AuthorizationMiddleware> _middleware;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public UserServiceTest()
        {
            _userRepository = new Mock<IUserRepository>();
            _authenticationService = new Mock<IAuthenticationService>();
            _middleware = new Mock<AuthorizationMiddleware>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
        }


        [Fact]
        public async Task AuthenticateUserLogin_ShouldValidateUser_AndReturnAccessToken()
        {
            //Arrange
            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>()
               {
                    {"X_API_KEY", "your_api_key"},
                    {"SOURCE_IP", "127.0.0.1"}
               })
               .Build();

            User mockUser = new()
            {
                Id = 1,
                Firstname = "John",
                Lastname = "Doe",
                Email = "info@dwittech.com",
                PhoneNumber = "0802333337",
                AddressLine1 = "Allen",
                AddressLine2 = "Sonubi",
                Country = "Nigeria",
                State = "Lagos",
                City = "Ogba",
                PostalCode = "21356",
                ZipCode = "6564536",
                Password = "JeSusIsLord",
                Status = Data.Enum.UserStatus.Active
            };

            var email = "john.doe@example.com";
            var hashedPassword = "hashed_password";
            var expectedToken = new TokenModel { AccessToken = "access_token" };
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.Headers["API_KEY"] = "your_api_key";

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockUserRepository = new Mock<IUserRepository>();
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            var mockNext = new Mock<RequestDelegate>();
            var mockAuthorizationMiddleware = new Mock<AuthorizationMiddleware>(mockNext.Object, configuration);

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            mockUserRepository.Setup(x => x.ValidateLogin(email, hashedPassword)).ReturnsAsync(true);
            mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(mockUser);
            mockAuthenticationService.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).ReturnsAsync(expectedToken);

            var userService = new UserService(mockUserRepository.Object, configuration, mockAuthenticationService.Object, mockAuthorizationMiddleware.Object, mockHttpContextAccessor.Object);

            // Act
            var token = await userService.AuthenticateUserLogin(email, hashedPassword);

            // Assert
            Assert.Equal(expectedToken, token);
            mockUserRepository.Verify(x => x.ValidateLogin(email, hashedPassword), Times.Once);
            mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
            mockAuthenticationService.Verify(x => x.GenerateAccessToken(It.IsAny<User>()), Times.Once);
        }
    }
}
