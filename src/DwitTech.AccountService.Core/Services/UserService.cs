using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DwitTech.AccountService.Core.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthenticationRepository _authRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IActivationService _activationService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
            IAuthenticationRepository authRepository,
            ILogger<UserService> logger, 
            IActivationService activationService, 
            IEmailService emailService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authRepository = authRepository;
            _logger = logger;
            _activationService = activationService;
            _emailService = emailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<Data.Entities.Role> GetAssignedRole(UserDto user)
        {
            var roles = await _roleRepository.GetRoles();
            if (user.Roles.ToString() is not null)
            {
                var assignedRole = roles.FirstOrDefault(x => x.Id == ((int)user.Roles));
                if (assignedRole != null)
                {
                    return assignedRole;
                }
            }
            return roles.First(x => x.Id == 2);

        }


        private Email GenerateEmail(UserDto user)
        {
            return new Email
            {
                FromEmail = _configuration["GMAIL_INFO:EMAIL"],
                ToEmail = user.Email,
                Body = "",
                Subject = "User Activation Email"
            };
        }


        private User GetUserEntity(UserDto user, Data.Entities.Role userIdentifiedRole)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userIdentifiedRole,
                Country = user.Country,
                State = user.State,
                Email = user.Email,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                ZipCode = user.ZipCode,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber,
                City = user.City
            };
        }

        private UserLogin GenerateLoginCredentials(UserDto user, int id)
        {

            return new UserLogin
            {
                Username = user.Email,
                Password = StringUtil.HashString(user.PassWord),
                UserId = id
            };
        }


        public async Task<bool> CreateUser(UserDto user)
        {
            try
            {
                Data.Entities.Role userRole = await GetAssignedRole(user);
                var userModel = GetUserEntity(user, userRole);
                var activationEmailHtmlTemplate = "ActivationEmailTemplate.html";
                var recipientName = $"{userModel.FirstName.ToLower()} {userModel.LastName.ToLower()}";
                var emailModel = GenerateEmail(user);
                await _activationService.SendActivationEmail(userModel.Id,recipientName, emailModel, activationEmailHtmlTemplate);
                var newUserId = await _userRepository.CreateUser(userModel);
                var loginCredentials = GenerateLoginCredentials(user, newUserId);
                await _userRepository.CreateUserLogin(loginCredentials);
                _logger.LogInformation(1, $"Login Credentials for the user with ID {userModel.Id} is successfully created");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"This error is due to {ex.Message}");
            }
        }


        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if(string.IsNullOrEmpty(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentNullException("Invalid password!");
            }

            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                throw new NullReferenceException("Email is not present in this context.");
            }

            var currentPasswordHash = StringUtil.HashString(currentPassword);
            var newPasswordHash = StringUtil.HashString(newPassword);
            var verifyLogin = await _authRepository.ValidateLogin(userEmail, currentPasswordHash);
            if (!verifyLogin)
            {
                throw new ArgumentException("Invalid current password");
            }

            var user = await _authRepository.GetUserByEmail(userEmail);

            if (currentPasswordHash == newPasswordHash)
            {
                throw new ArgumentException("Passwords are Identical!");
            }

            await _userRepository.UpdateUserLoginAsync(user, newPasswordHash);
            _logger.LogInformation($"Password for the user with ID {user.Id} was changed successfully");
            return true;
        }
    }
}
