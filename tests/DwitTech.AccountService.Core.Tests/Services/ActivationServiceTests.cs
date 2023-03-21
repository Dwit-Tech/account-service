using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {

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
            
            var actService = new ActivationService(configuration, repository);

            //Act
            var actual = actService.SendActivationEmail(fromMail, toMail, templateName, RecipientName);

            //Assert
            Assert.Equal(expected, actual);
        }

        private readonly IConfiguration _configuration;

        [Fact]
        public async Task ActivateUser_ValidatesActivationCodeAndSendsWelcomeEmail_WhenCalled()
        {
            //Arrange
            var mockValidationCode = new ValidationCode
            {
                Id = 01,
                UserId = 1,
                Code = "erg3345dh2"
            };

            var mockUserRepository = new Mock<IUserRepository>();


            mockUserRepository.Setup(x => x.GetUserActivationDetail(It.IsAny<string>())).ReturnsAsync(mockValidationCode);
            mockUserRepository.Setup(x => x.GetUserStatus(It.IsAny<int>())).ReturnsAsync(true);
            mockUserRepository.Setup(x => x.ValidateUserActivationCodeExpiry(It.IsAny<string>())).ReturnsAsync(true);

            var actService = new ActivationService(_configuration, mockUserRepository.Object);

            string activationCode = "erg3345dh2";
            var user = new User()
            {
                Firstname = "Jane",
                Lastname = "Doe",
                Email = "info@janedoe",
                Password = "reddanger"
            };
            string fromEmail = "support@gmail";
            string toEmail = "info@gmail.com";
            string templateName = "WelcomeEmail.html";
            string subject = "Account Details";
            string cc = "";
            string bcc = "";

            //Act
            var actual = await actService.ActivateUser(activationCode,user, fromEmail, toEmail, templateName, subject, cc, bcc);

            //Assert
            mockUserRepository.Verify(x => x.GetUserActivationDetail(activationCode), Times.Once);
            mockUserRepository.Verify(x => x.GetUserStatus(It.IsAny<int>()), Times.Once);
            mockUserRepository.Verify(x => x.ValidateUserActivationCodeExpiry(activationCode), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUserStatus(It.IsAny<ValidationCode>()), Times.Once);

        }
    }
}
