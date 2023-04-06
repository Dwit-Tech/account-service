using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Middleware;
using DwitTech.AccountService.Core.Models;
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
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _authenticationService;
        private readonly AuthorizationMiddleware _middleware;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository,
            IConfiguration configuration,IAuthenticationService authenticationService, AuthorizationMiddleware middleware, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _middleware = middleware;
            _httpContextAccessor = httpContextAccessor;
        }

        private static bool IsUserActivated(User user)
        {
            if (user.Status == Data.Enum.UserStatus.Active)
            {
                return true;
            }
            return false;
        }
        public async Task<TokenModel> AuthenticateUserLogin (string email, string hashedPassword)
        {
            var context = _httpContextAccessor.HttpContext;

            //validate user's source Ip and Api Key
            await _middleware.InvokeAsync(context);

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
            var token = await _authenticationService.GenerateAccessToken(user);
            return token;
        }
    }
}
