using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
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

            var mockDbContext = new Mock<AccountDbContext>();

            IUserRepository repository = new UserRepository(mockDbContext.Object);


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
            var inMemorySettings = new Dictionary<string, string>
            {
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockDbContext = new Mock<AccountDbContext>();

            IUserRepository repository = new UserRepository(mockDbContext.Object);

            var actService = new ActivationService(configuration, repository);

            
            string activationCode = "erg3345dh2";
            string fromEmail = "support@gmail";
            string toEmail = "info@gmail.com";
            string templateName = "WelcomeEmail";
            string subject = "Account Details";
            string cc = "";
            string bcc = "";

            //Act
            var actual = await actService.ActivateUser(activationCode, fromEmail, toEmail, templateName, subject, cc, bcc);

            //Assert
            Assert.True(actual);

        }
    }
}
