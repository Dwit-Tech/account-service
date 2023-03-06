using DwitTech.AccountService.Core.Services;
using Microsoft.Extensions.Configuration;
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
        public void SendActivationEmail_Returns_True(string fromMail, string toMail, string templateName, string RecipientName, bool expected)
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseUrl", "https://example.com"}
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var actService = new ActivationService(configuration);

            //Act
            var actual = actService.SendActivationEmail(fromMail, toMail, templateName, RecipientName);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
