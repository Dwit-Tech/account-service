using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Sockets;
using static DwitTech.AccountService.Data.Repository.IUserRepository;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {
        private readonly IConfiguration _configuration;
        private IActivationService actService;
        private User mockUser;
        private ValidationCode mockValidationCode;
        private Mock<IUserRepository> mockUserRepository;


        [Theory]
        [InlineData("testcase@gmail.com", "example@gmail.com", "EmailTemplate.html", "Mike", true)]
        public void SendActivationEmail_Returns_True(string fromMail, string toMail, string templateName,string RecipientName, bool expected)
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseUrl", "https://example.com"}
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            IUserRepository repository = new UserRepository(mockDbContext.Object);
            var iConfig = new Mock<IConfiguration>();
            var actService = new ActivationService(configuration, repository);

            //Act
            var actual = actService.SendActivationEmail(fromMail, toMail, templateName, RecipientName);

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public async Task ActivateUser_Returns_BooleanResult()
        {
            //Arrange
            var mockValidationCode = new ValidationCode
            {
                Id = 01,
                UserId = 1,
                Code = "activationCode",
                Channel = 27,
                CodeType = 2,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            


            mockUserRepository.Setup(x => x.GetActivationDetail(It.IsAny<string>())).ReturnsAsync(mockValidationCode);
            mockUserRepository.Setup(x => x.GetUserStatus(It.IsAny<int>())).ReturnsAsync(true);
            mockUserRepository.Setup(x => x.ValidateActivationCodeExpiry(It.IsAny<string>())).ReturnsAsync(true);

            var actService = new ActivationService(_configuration, mockUserRepository.Object);

            string activationCode = "erg3345dh2";
            string fromEmail = "support@gmail";
            string toEmail = "info@gmail.com";
            string templateName = "WelcomeEmail.html";
            string subject = "Account Details";
            string cc = "";
            string bcc = "";

            //Act
            var actual = await actService.ActivateUser(activationCode, fromEmail, toEmail, templateName, subject, cc, bcc);

            //Assert
            mockUserRepository.Verify(x => x.GetActivationDetail(activationCode), Times.Once);
            mockUserRepository.Verify(x => x.GetUserStatus(It.IsAny<int>()), Times.Once);
            mockUserRepository.Verify(x => x.ValidateActivationCodeExpiry(activationCode), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUserStatus(It.IsAny<ValidationCode>()), Times.Once);

        }
    }
}
