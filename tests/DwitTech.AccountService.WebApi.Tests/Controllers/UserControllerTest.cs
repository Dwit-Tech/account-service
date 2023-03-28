using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DwitTech.AccountService.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController _controller;
        private readonly IHttpClientFactory _httpClientFactory;

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
                { "FROM_EMAIL","example@gmail.com" }

            }).Build();

            var iHttpClientFactory = new Mock<IHttpClientFactory>();
            var _endpointUrl = "https://jsonplaceholder.typicode.com/posts";
            var _mockService = new ActivationService(_configuration, userRepository.Object, iHttpClientFactory.Object, _endpointUrl);
            UserController userController = new UserController(_mockService);
            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}