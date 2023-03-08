using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IActivationService> _mockActService;
        private readonly UserController _controller;
        private readonly IConfiguration _configuration;

        [Fact]
        public void ActivateUser_ShouldReturn_HTTP200()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockDbContext = new Mock<AccountDbContext>();
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var iConfig = new Mock<IConfiguration>();

            var _mockService = new Mock<ActivationService>(iConfig.Object, userRepository.Object);

            var userController = new UserController(_mockService.Object);

            string activationCode = "erg3345dh2";

            //act
            var actual = userController.ActivateUser(activationCode);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}
