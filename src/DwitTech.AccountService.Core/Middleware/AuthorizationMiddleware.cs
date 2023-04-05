using Microsoft.AspNetCore.Http;
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
        private readonly string _apiKey;
        private readonly string[] _allowedIpAddresses;

        public AuthorizationMiddleware(RequestDelegate next, string apiKey, string[] allowedIpAddresses)
        {
            _next = next;
            _apiKey = apiKey;
            _allowedIpAddresses = allowedIpAddresses;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //validates api key
            var apiKey = context.Request.Headers["API_KEY"];

            if (apiKey != _apiKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API key.");
                return;
            }

            //validates sourceIp
            var remoteIpAddress = context.Connection.RemoteIpAddress;

            if (!Array.Exists(_allowedIpAddresses, ip => ip == remoteIpAddress.ToString()))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Source IP not allowed.");
                return;
            }

            await _next(context);
        }
    }
}
