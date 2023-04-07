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
        public async Task InvokeAsync_WithValidApiKeyAndSourceIp_ReturnsOk()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.Headers["API_KEY"] = "your_api_key";

            var middleware = new AuthorizationMiddleware(context => Task.CompletedTask, _configuration);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            var middleware = new AuthorizationMiddleware(context => Task.CompletedTask, _configuration);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidSourceIp_ReturnsUnauthorized()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = "your_api_key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");

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
            context.Request.Headers["API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");

            var middleware = new AuthorizationMiddleware(context => Task.CompletedTask, _configuration);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }
    }
}