using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {
        [Theory]
        [InlineData("testcase@gmail.com", "example@gmail.com", "EmailTemplate.html", "Mike", true)]
        public async Task SendActivationEmail_Returns_True(string fromMail, string toMail, string templateName, string RecipientName, bool expected)
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseUrl", "https://example.com"},
                {"NotificationService:BaseUrl","https://example.com" },
                {"NotificationService:SendEmail", "/send"},
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
          
            var mockDbContext = new Mock<AccountDbContext>();

            IUserRepository repository = new UserRepository(mockDbContext.Object);


            ActivationService actService = new ActivationService(configuration, repository);

            //Act
            var actual = await actService.SendActivationEmail(Guid.NewGuid().ToString(), fromMail, toMail, templateName, RecipientName);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
  