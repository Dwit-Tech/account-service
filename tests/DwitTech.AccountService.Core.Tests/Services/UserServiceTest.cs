using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<RequestDelegate> _next;
        private readonly Mock<IAuthenticationService> _authenticationService;

        [Fact]
        public async Task UserLogin_Returns_BooleanResult()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"API_KEY", "info@dwittech.com"}
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockUserRepository = new Mock<IUserRepository>();
            var authenticationService = new Mock<AuthenticationService>();
            var next = new Mock<RequestDelegate>();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            mockUserRepository.Setup(x => x.ValidateLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var userService = new UserService(mockUserRepository.Object, next.Object, configuration, authenticationService.Object, httpContextAccessor.Object);

            string email = "hello@support.com";
            string hashedPassword = "whgwygy37t63t36shhcxvw";
            //Act
            var actual = await userService.UserLogin(email, hashedPassword);

            //Assert
            mockUserRepository.Verify(x => x.ValidateLogin(email, hashedPassword), Times.Once);
            Assert.IsType<bool>(actual);

        }
    }
}
