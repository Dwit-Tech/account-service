using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {
        private const string activationEmailTemplateName = "ActivationEmailTemplate.html";

        [Fact(Skip = "method is disabled")]
        public async Task SendActivationEmail_ShouldCall_SendMailAsync_WithCorrectEmailParameters()
        {
            // Arrange
            Environment.SetEnvironmentVariable("BASE_URL", "http://localhost:5000");
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", "http://localhost:5001");
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", "sendmail");

            var _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var email = new Email();
            var subject = "Account Activation";
            var activationCode = "activationCode";
            var userId = 1;
            var recipientName = "John Doe";
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockEventPublisher = new Mock<IEventPublisher>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userRepository = new Mock<IUserRepository>();
            var iEmailMock = new EmailService(_configuration, mockHttpClientFactory.Object, mockEventPublisher.Object);


            userRepository.Setup(x => x.SaveUserValidationCode(It.IsAny<ValidationCode>()))
                .Verifiable();
            var activationService = new ActivationService(_configuration, userRepository.Object, iEmailMock, mockHttpClientFactory.Object);

            mockHttpMessageHandler.Protected() //Mock the HTTP response
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = null
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_configuration["NOTIFICATION_SERVICE_BASE_URL"])
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await activationService.SendActivationEmail(userId, recipientName, email, activationEmailTemplateName);

            // Assert
            userRepository.Verify(x => x.SaveUserValidationCode(It.IsAny<ValidationCode>()), Times.Once);

            mockHttpClientFactory.Verify(_ => _.CreateClient(It.IsAny<string>()), Times.Once);

            var capturedRequest = mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (req, token) =>
            {
                // Assert
                var body = req.Content != null ? await req.Content.ReadAsStringAsync() : null;
                Assert.NotNull(body);
                Assert.Contains(subject, body);
                Assert.Contains(recipientName, body);
                Assert.Contains(_configuration["BASE_URL"] + "/Account/Activation/" + activationCode, body);
            });
            Assert.True(result);

            // Remove environment variables
            Environment.SetEnvironmentVariable("BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_BASE_URL", null);
            Environment.SetEnvironmentVariable("NOTIFICATION_SERVICE_SENDMAIL_END_POINT", null);
        }
    
    }
}