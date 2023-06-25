using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Moq;
using System.Net;
using DwitTech.AccountService.Core.Interfaces;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class EmailServiceTest
    {
        [Fact(Skip = "method is disabled")]
        public async Task SendHttpEmailAsync_ShouldReturnTrue_WhenMailSendingIsSuccessful()
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

            var mockEventPublisher = new Mock<IEventPublisher>();
            mockEventPublisher
                .Setup(p => p.PublishEmailEventAsync("email-sent", email))
                .ReturnsAsync(true);

            var emailService = new EmailService(_configuration, mockHttpClientFactory.Object, mockEventPublisher.Object);

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            Assert.True(result);
            mockEventPublisher.Verify(p => p.PublishEmailEventAsync("email-sent", email), Times.Never);

            // Remove environment variables
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", null);
        }

        [Fact(Skip = "method is disabled")]
        public async Task SendHttpEmailAsync_ShouldReturnFalse_WhenMailSendingFailed()
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

            var mockEventPublisher = new Mock<IEventPublisher>();
            mockEventPublisher
                .Setup(p => p.PublishEmailEventAsync("email-sent", email))
                .ReturnsAsync(true);

            var emailService = new EmailService(_configuration, mockHttpClientFactory.Object, mockEventPublisher.Object);          
            
            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            mockEventPublisher.Verify(p => p.PublishEmailEventAsync("email-sent", email), Times.Once);

            // Remove environment variables
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", null);
        }

        [Fact]
        public async Task SendMailAsync_CallsPublishEmailEventAsync_WhenUseHttpIsFalse()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var email = new Email { };
            var eventPublisherMock = new Mock<IEventPublisher>();
            eventPublisherMock
                .Setup(p => p.PublishEmailEventAsync("email-sent", email))
                .ReturnsAsync(true);

            var emailService = new EmailService(mockConfiguration.Object, mockHttpClientFactory.Object, eventPublisherMock.Object);

            // Act
            var result = await emailService.SendMailAsync(email);

            // Assert
            Assert.True(result);
            eventPublisherMock.Verify(p => p.PublishEmailEventAsync("email-sent", email), Times.Once);
        }

        [Fact(Skip = "method is disabled")]
        public async Task SendMailAsync_DoesNotCallPublishEmailEventAsync_WhenUseHttpIsTrue_AndSendHttpEmailAsyncReturnsTrue()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var email = new Email { };
            var eventPublisherMock = new Mock<IEventPublisher>();
            eventPublisherMock
                .Setup(p => p.PublishEmailEventAsync("email-sent", email))
                .ReturnsAsync(true);

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var emailService = new EmailService(mockConfiguration.Object, mockHttpClientFactory.Object, eventPublisherMock.Object);            

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            Assert.True(result);
            eventPublisherMock.Verify(p => p.PublishEmailEventAsync("email-sent", email), Times.Never);
        }

        [Fact(Skip = "method is disabled")]
        public async Task SendMailAsync_CallsPublishEmailEventAsync_WhenUseHttpIsTrue_AndSendHttpEmailAsyncReturnsFalse()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var email = new Email { };
            var eventPublisherMock = new Mock<IEventPublisher>();
            eventPublisherMock
                .Setup(p => p.PublishEmailEventAsync("email-sent", email))
                .ReturnsAsync(true);

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.RequestTimeout));

            var mockHttpClient = new HttpClient(mockMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var emailService = new EmailService(mockConfiguration.Object, mockHttpClientFactory.Object, eventPublisherMock.Object);

            // Act
            var result = await emailService.SendMailAsync(email, useHttp: true);

            // Assert
            Assert.True(result);
            eventPublisherMock.Verify(p => p.PublishEmailEventAsync("email-sent", email), Times.Once);
        }
    }
}
