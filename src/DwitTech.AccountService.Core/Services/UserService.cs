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
using AutoMapper;

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
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
            IAuthenticationRepository authRepository,
            ILogger<UserService> logger, 
            IActivationService activationService, 
            IEmailService emailService,
            IConfiguration configuration,
            IAuthenticationService authenticationService,            
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authRepository = authRepository;
            _logger = logger;
            _activationService = activationService;
            _emailService = emailService;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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

            if (currentPasswordHash == newPasswordHash)
            {
                throw new ArgumentException("Passwords are Identical!");
            }

            var user = await _userRepository.GetUserByEmail(userEmail);            

            await _userRepository.UpdateUserLoginAsync(user, newPasswordHash);
            _logger.LogInformation(1, $"Password for the user with ID {user.Id} was changed successfully");
            return true;
        }

        public async Task<bool> LogoutUser(string authHeader)
        {
            try
            {
                var userId = JwtUtil.GenerateIdFromToken(authHeader);
                var logoutResult = await _authenticationService.DeleteUserToken(userId);
                return logoutResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task<bool> EditAccount(EditRequestDto editDto)
        {
            var editResult = await PerformEdit(editDto);
          
            if(editResult)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> PerformEdit(EditRequestDto editDto)
        {
            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail == null)
            {
                throw new NullReferenceException("Email is not present in this context.");
            }

            var user = await _userRepository.GetUserByEmail(userEmail);
            if (user != null)
            {

                if (!string.Equals(user.Email, editDto.Email, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.Email))
                {
                    user.Email = editDto.Email;
                }

                if (!string.Equals(user.FirstName, editDto.FirstName, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.FirstName))
                {
                    user.FirstName = editDto.FirstName;
                }

                if (!string.Equals(user.LastName, editDto.LastName, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.LastName))
                {
                    user.LastName = editDto.LastName;
                }

                if (!string.Equals(user.AddressLine1, editDto.AddressLine1, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.AddressLine1))
                {
                    user.AddressLine1 = editDto.AddressLine1;
                }

                if (!string.Equals(user.AddressLine2, editDto.AddressLine2, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.AddressLine2))
                {
                    user.AddressLine2 = editDto.AddressLine2;
                }

                if (!string.Equals(user.PhoneNumber, editDto.PhoneNumber, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.PhoneNumber))
                {
                    user.PhoneNumber = editDto.PhoneNumber;
                }

                if (!string.Equals(user.Country, editDto.Country, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.Country))
                {
                    user.Country = editDto.Country;
                }

                if (!string.Equals(user.State, editDto.State, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.State))
                {
                    user.State = editDto.State;
                }

                if (!string.Equals(user.PostalCode, editDto.PostalCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.PostalCode))
                {
                    user.PostalCode = editDto.PostalCode;
                }

                if (!string.Equals(user.ZipCode, editDto.ZipCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(editDto.ZipCode))
                {
                    user.ZipCode = editDto.ZipCode;
                }

                await _userRepository.UpdateUser(user);
                _logger.LogInformation("Account was edited successfully");
                return true;
            }
            else
            {
                _logger.LogError("Error encountered while editing account");
                return false;
            }
        }
    }
}