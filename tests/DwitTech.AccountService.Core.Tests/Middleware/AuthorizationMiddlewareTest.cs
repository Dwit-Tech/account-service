using DwitTech.AccountService.Core.Middleware;
using Microsoft.AspNetCore.Http;
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
        public async Task InvokeAsync_WithValidApiKeyAndSourceIp_CallsNextMiddleware()
        {
            // Arrange
            var apiKey = "VALID_API_KEY";
            var allowedIpAddresses = new[] { "192.168.1.1", "192.168.1.2" };

            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = apiKey;
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");

            var mockNext = new Mock<RequestDelegate>();

            var middleware = new AuthorizationMiddleware(mockNext.Object, apiKey, allowedIpAddresses);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var apiKey = "VALID_API_KEY";
            var allowedIpAddresses = new[] { "192.168.1.1", "192.168.1.2" };

            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");

            var mockNext = new Mock<RequestDelegate>();

            var middleware = new AuthorizationMiddleware(mockNext.Object, apiKey, allowedIpAddresses);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
            mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidSourceIp_ReturnsUnauthorized()
        {
            // Arrange
            var apiKey = "VALID_API_KEY";
            var allowedIpAddresses = new[] { "192.168.1.1", "192.168.1.2" };

            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = apiKey;
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");

            var mockNext = new Mock<RequestDelegate>();

            var middleware = new AuthorizationMiddleware(mockNext.Object, apiKey, allowedIpAddresses);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
            mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidSourceIpAndApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var apiKey = "VALID_API_KEY";
            var allowedIpAddresses = new[] { "192.168.1.1", "192.168.1.2" };

            var context = new DefaultHttpContext();
            context.Request.Headers["API_KEY"] = "invalid-api-key";
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.3");

            var mockNext = new Mock<RequestDelegate>();

            var middleware = new AuthorizationMiddleware(mockNext.Object, apiKey, allowedIpAddresses);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
            mockNext.Verify(next => next(context), Times.Never);
        }
    }
}