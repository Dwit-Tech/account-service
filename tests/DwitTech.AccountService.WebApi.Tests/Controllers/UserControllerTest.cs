using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DwitTech.AccountService.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {


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
            var _mockService = new ActivationService(_configuration,userRepository.Object, iEmailService.Object,iHttpClientFactory.Object);
            var userService = new Mock<UserService>(_userRepository.Object, _roleRepository.Object, _logger.Object, _activationService.Object, _emailService.Object);
            var userController = new UserController(_mockService, userService.Object);

            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }


        [Fact]
        public async Task CreateUser_Should_Return_True_If_TaskCompletes_Successfully()
        {

            var _userRepository = new Mock<IUserRepository>();
            var _roleRepository = new Mock<IRoleRepository>();
            var _logger = new Mock<ILogger<UserService>>();
            var _activationService = new Mock<IActivationService>();
            var _emailService = new Mock<IEmailService>();

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

            var userController = new UserController(_activationService.Object, userService.Object);
            var result = userController.CreateUser(userDto);
            Assert.True(result.IsCompleted);
        }
    }
}