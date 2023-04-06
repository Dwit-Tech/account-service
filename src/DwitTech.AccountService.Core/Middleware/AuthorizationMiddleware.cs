using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string apiKey = _configuration["X_API_KEY"];
            string sourceIP = _configuration["SOURCE_IP"];

            // validate source IP
            if (context.Connection.RemoteIpAddress.ToString() != sourceIP)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized access");
                return;
            }

            // validate API key
            if (!context.Request.Headers.ContainsKey("API_KEY") || context.Request.Headers["API_KEY"] != apiKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }

            await _next(context);
        }
    }
}
