using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DwitTech.AccountService.WebApi.Controllers
{
    public class UserController : BaseController
    {

        private readonly IActivationService _activationService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public UserController(IUserService userService, IActivationService activationService, IConfiguration configuration)
        {
            _userService = userService;
            _activationService = activationService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto user)
        {

           
            try
            {
                user.Password = StringUtil.HashString(user.Password);
                user.Roles = _userService.CheckUserRoleState(user.Roles);
                var recipientName = $"{user.Firstname} {user.Lastname}";
                _activationService.SendActivationEmail(_configuration["GmailInfo:Email"], user.Email, "EmailTemplate.html", recipientName.ToUpper(), "", "", "");
                var newUser = await _userService.CreateUser(user);
                return Ok(newUser);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
