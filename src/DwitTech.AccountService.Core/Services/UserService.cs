using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace DwitTech.AccountService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _authenticationService;
        private const string ApiKeyHeaderName = "X_API_KEY";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, RequestDelegate next, 
            IConfiguration configuration,IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _next = next;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task GetUserSourceIp(HttpContext context)
        {
            string clientIP = context.Response.HttpContext.Connection.RemoteIpAddress.ToString();

            if (clientIP == "::1")
            {
                clientIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[2].ToString();
                return;
            }
            await _next(context);
        }
        private async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out
                var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api key is missing");
                
            }

            var apiKey = _configuration["X_API_KEY"];
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Api key");
                
            }
            
            await _next(context);
        }
        private static bool IsUserActivated(User user)
        {
            if (user.Status == Data.Enum.UserStatus.Active)
            {
                return true;
            }
            return false;
        }
        public async Task<Task<Models.TokenModel>> UserLogin (string email, string hashedPassword)
        {
            var context = _httpContextAccessor.HttpContext;

            // Check user's source IP
            var userIp = GetUserSourceIp(context);
            if (userIp == null)
            {
                throw new Exception("Invalid IP address.");
            }

            // Check user's API key
            await InvokeAsync(context);

            //validate email and password combination
            var dbUser = _userRepository.ValidateLogin(email, hashedPassword);
            hashedPassword = StringUtil.HashString(hashedPassword);
            if (dbUser == null)
            {
                throw new Exception("Email or Password is incorrect.");
            }

            //check user status
            var user = await _userRepository.GetUserByEmail(email);
            if (!IsUserActivated(user))
            {
                throw new Exception("User is inactive.");
            }

            // create authentication token
            var token = _authenticationService.GenerateAccessToken(user);
            return token;
        }
    }
}
