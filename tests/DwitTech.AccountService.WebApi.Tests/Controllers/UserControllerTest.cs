using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IActivationService> _mockActService;
        private readonly UserController _controller;

        [Fact]
        public void ActivateUser_ShouldReturn_HTTP200()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            var userRepository = new Mock<UserRepository>(mockDbContext.Object);
            var iConfig = new Mock<IConfiguration>();

            var _mockService = new Mock<ActivationService>(iConfig.Object, userRepository.Object);

            var userController = new UserController(_mockService.Object);

            string activationCode = "erg3345dh2";
            var user = new User()
            {
                Firstname = "Jane",
                Lastname = "Doe",
            };
            string fromEmail = "support@gmail";
            string toEmail = "info@gmail.com";
            string templateName = "WelcomeEmail";
            string subject = "Welcome message";
            string cc = "";
            string bcc = "";

            //act
            var actual = userController.ActivateUser(activationCode, user, fromEmail, toEmail, templateName, subject, cc, bcc);

            //assert
            Assert.True(actual.IsCompletedSuccessfully);
        }
    }
}
