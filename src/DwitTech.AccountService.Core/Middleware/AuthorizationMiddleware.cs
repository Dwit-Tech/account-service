﻿using Microsoft.AspNetCore.Http;
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
            if (context.Request.Path.Value.ToLower().Contains("health"))
            {
                await _next(context);
                return;
            }

            string apiKeys = _configuration["X_API_KEYS"];

            // validate API key
            if (!context.Request.Headers.ContainsKey("X_API_KEY") || !apiKeys.Contains(context.Request.Headers["X_API_KEY"]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }

            await _next(context);
        }
    }
}
