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

            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "valid-key";

            // Act
            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "valid-key";
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidApiKey_ShouldReturnUnauthorized()
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

            var context = new DefaultHttpContext();
            context.Request.Headers["X_API_KEY"] = "invalid-key";
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.Equal("Invalid API key", responseBody);
        }

        [Fact]
        public async Task InvokeAsync_WithPathContainingHealth_ShouldCallNextMiddleware()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var middleware = new AuthorizationMiddleware(
                (innerHttpContext) => Task.FromResult(0),
                configuration.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/health";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
}