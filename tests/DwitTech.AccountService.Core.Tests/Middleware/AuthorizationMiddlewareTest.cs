using DwitTech.AccountService.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Middleware
{
    public class AuthorizationMiddlewareTest
    {
        private readonly IConfiguration _configuration;
        public AuthorizationMiddlewareTest()
        {
            _configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>()
               {
                    {"X_API_KEY", "your_api_key"},
                    {"SOURCE_IP", "127.0.0.1"}
               })
               .Build();
        }


        [Fact]
        public async Task InvokeAsync_WithValidApiKey_ShouldCallNextMiddleware()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "X_API_KEYS", "valid-key" }
                })
                .Build();

            var middleware = new AuthorizationMiddleware(
                (innerHttpContext) => Task.FromResult(0),
                configuration);

            // Act
            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "valid-key";
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            var middleware = new AuthorizationMiddleware(context => Task.CompletedTask, _configuration);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidSourceIpAndApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");

            var middleware = new AuthorizationMiddleware(context => Task.CompletedTask, _configuration);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }
    }
}