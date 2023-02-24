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
        public void SendActivationEmail_Returns_True(string fromMail, string toMail, string templateName, string RecipientName, bool expected)
        {
            //Arrange
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
    }
}
