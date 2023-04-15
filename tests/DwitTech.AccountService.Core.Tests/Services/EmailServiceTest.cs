using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class EmailServiceTest
    {
        [Fact]
        public async Task SendMailAsync_ShouldReturnTrue_WhenMailSendingIsSuccessful()
        {
            // Arrange
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", "http://localhost:5001");
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "sendmail");

            var _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var email = new Email();
            var userRepository = new Mock<IUserRepository>();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);
            var iEmailMock = new EmailService(_configuration, mockHttpClientFactory.Object);

            // Act
            var result = await iEmailMock.SendMailAsync(email);

            // Assert
            Assert.True(result);

            // Remove environment variables
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", null);
        }

        [Fact]
        public async Task SendMailAsync_ShouldReturnFalse_WhenMailSendingFailed()
        {
            // Arrange
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", "http://localhost:5001");
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "sendmail");

            var _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var email = new Email();
            var userRepository = new Mock<IUserRepository>();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);
            var iEmailMock = new EmailService(_configuration, mockHttpClientFactory.Object);

            // Act
            var result = await iEmailMock.SendMailAsync(email);

            // Assert
            Assert.False(result);

            // Remove environment variables
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", null);
        }
    }
}
