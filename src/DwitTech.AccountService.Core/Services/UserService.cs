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
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _logger = logger;
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

        private User CustomMapper(UserDto user, Data.Entities.Role userIdentifiedRole)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = userIdentifiedRole,
                Country = user.Country,
                State = user.State,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                ZipCode = user.ZipCode,
                PostalCode = user.PostalCode,
                PassWord = user.PassWord
            };
        }

        public async Task CreateUser(UserDto user)
        {
            try
            {
               
                Data.Entities.Role userRole = await GetAssignedRole(user);
                var userModel = CustomMapper(user, userRole);
                userModel.PassWord = StringUtil.HashString(userModel.PassWord);
                await _userRepository.CreateUser(userModel);

                //return createdUser;
                _logger.LogInformation(2, $" This is from NLogger {userRole.Id}, {userRole.Name}");
                //return createdUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"This error is due to {ex.Message}");
            }
        }

       
    }
}
