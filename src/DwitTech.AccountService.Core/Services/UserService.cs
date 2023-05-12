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
using DwitTech.AccountService.Data.Enum;
using Microsoft.EntityFrameworkCore;

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

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
            IAuthenticationRepository authRepository,
            ILogger<UserService> logger,
            IActivationService activationService,
            IEmailService emailService,
            IConfiguration configuration,
            IAuthenticationService authenticationService,
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
            _authenticationService = authenticationService;
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
                City = user.City,
                Status = UserStatus.Inactive
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

            var newUserId = await _userRepository.CreateUser(userModel);
            await _activationService.SendActivationEmail(newUserId, recipientName, emailModel, activationEmailHtmlTemplate);

            var loginCredentials = GenerateLoginCredentials(user, newUserId);

            await _userRepository.CreateUserLogin(loginCredentials);

            _logger.LogInformation(1, $"Login Credentials for the user with ID {userModel.Id} is successfully created");

            return true;

        }


        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
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

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
            }
            
            catch (DbUpdateException ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error deleting user with ID {id}: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetResetPasswordUrl(int userId)
        {
            string baseUrl = _configuration["BASE_URL"];
            string resetToken = Guid.NewGuid().ToString();
            string resetPasswordUrl = baseUrl + "/Account/Reset-Password/" + resetToken;
            
            var existingValidatonCode = await _userRepository.FindUserValidationCode(userId, CodeType.ResetToken);

            if (existingValidatonCode != null)
            {
                existingValidatonCode.Code = resetToken;
                existingValidatonCode.ModifiedOnUtc = DateTime.UtcNow;
                await _userRepository.UpdateValidationCode(existingValidatonCode);
                _logger.LogInformation("ValidationCode for the user with ID {UserId} was updated successfully", userId);
                return resetPasswordUrl;
            }
                    
            var validationCode = new ValidationCode
            {
                Code = resetToken,
                CodeType = CodeType.ResetToken,
                UserId = userId,
                NotificationChannel = NotificationChannel.Email,
                ModifiedOnUtc = DateTime.UtcNow
            };
            await _userRepository.SaveUserValidationCode(validationCode);
            _logger.LogInformation("ValidationCode for the user with ID {userId} was added successfully", userId);
            return resetPasswordUrl;
        }

        private async Task<bool> SendResetPasswordEmail(User user, string resetPasswordUrl)
        {
            string templateText = await _activationService.GetTemplate("ResetPasswordEmailTemplate.html");
            templateText = templateText.Replace("{{firstName}}", user.FirstName);
            templateText = templateText.Replace("{{lastName}}", user.LastName);
            templateText = templateText.Replace("{{resetPasswordUrl}}", resetPasswordUrl);
            string body = templateText;
            const string subject = "Password Reset";
            string fromEmail = _configuration["FROM_EMAIL"];
            var email = new Email { FromEmail = fromEmail, ToEmail = user.Email, Subject = subject, Body = body };
            var response = await _emailService.SendMailAsync(email);
            return response;
        }

        public async Task<bool> ResetPassword(string userEmail)
        {
            var user = await _userRepository.GetUserByEmail(userEmail);
            if (user == null)
            {
                throw new ArgumentException("Invalid email address");
            }

            try
            {
                var resetPasswordUrl = await GetResetPasswordUrl(user.Id);
                return await SendResetPasswordEmail(user, resetPasswordUrl);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Error! Unable to update the database!");
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error! Unable to update the database!");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error! Unable to update the database!");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error! Unable to update the database!");
                throw;
            }
        }
    }
}