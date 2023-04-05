using AutoMapper;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DwitTech.AccountService.Core.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IActivationService _activationService;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserService> logger, IActivationService activationService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _activationService = activationService;
            _emailService = emailService;
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

        private User GetCustomMapper(UserDto user, Data.Entities.Role userIdentifiedRole)
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
                UserName = user.Email,
                PassWord = StringUtil.HashString(user.PassWord),
                UserId = id
            };
        }


        public async Task CreateUser(UserDto user)
        {
            try
            {
                Data.Entities.Role userRole = await GetAssignedRole(user);
                var userModel = GetCustomMapper(user, userRole);
                var activationEmailHtmlTemplate = "ActivationEmailTemplate.html";
                var recipientName = $"{userModel.FirstName.ToLower()} {userModel.LastName.ToLower()}";
                var emailModel = _emailService.GenerateEmail(user);
                await _activationService.SendActivationEmail(userModel.Id,recipientName, emailModel, activationEmailHtmlTemplate);
                await _userRepository.CreateUser(userModel);
                var loginCredentials = GenerateLoginCredentials(user, userModel.Id);
                await _userRepository.CreateUserLoginCredentials(loginCredentials);
                _logger.LogInformation(1, $"This is logged when the user with ID {userModel.Id} is successfully created");
            }
            catch (Exception ex)
            {
                throw new Exception($"This error is due to {ex.Message}");
            }
        }

        
    }
}
