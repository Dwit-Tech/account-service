using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {
        [Fact]
        public async Task SendActivationEmail_Returns_True()

        {
            var userId = 2;
            var templateName = "EmailTemplate.html";
            var recipientName = "Mike";

            var mockEmail = new Email()
            {
                FromEmail = "testcase@gmail.com",
                ToEmail = "example@gmail.com",
                Subject = "",
                Body = "",
                Cc = "",
                Bcc = ""
            };

            var _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "BASE_URL", "https://example.com" },
                { "NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "https://jsonplaceholder.typicode.com/posts"}
            }).Build();

            var userRepository = new Mock<IUserRepository>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });
            var client = new HttpClient(httpClientMock.Object);
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(client);

            IActivationService activationService = new ActivationService(_configuration, userRepository.Object, httpClientFactoryMock.Object);
            var result = await activationService.SendActivationEmail(userId, templateName, recipientName, mockEmail);
            Assert.True(result);
        }
    }
}