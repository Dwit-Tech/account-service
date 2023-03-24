using AutoMapper;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.IdentityModel.Tokens;

namespace DwitTech.AccountService.Core.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }
        public Role CheckUserRoleState(Role userRole)
        {
            
            if(userRole.Name.Trim().IsNullOrEmpty())
            {
                return new Role {Name="User", Description="User Role"};
            }


            var role = new Role();

            switch (userRole.Name.ToLower())
            {
                case "user":
                    
                    role.Name = "User";
                    role.Description = "User Role";
                    break;

                case "admin":
                   
                    role.Name = "Admin";
                    role.Description = "Administrator Role";
                    break;


            }
            
            return role;
        }

        public async Task CreateUser(UserDto user)
        {
            try
            {
                var userModel = _mapper.Map<User>(user);
                await _userRepository.CreateUser(userModel);
                //return createdUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"This error is due to {ex.Message}");
            }
        }

       
    }
}
