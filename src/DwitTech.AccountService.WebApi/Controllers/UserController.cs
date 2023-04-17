using Microsoft.AspNetCore.Authorization;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.WebApi.Controllers
{ 
    public class UserController : BaseController
    {
        private readonly IActivationService _activationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IActivationService activationService, IAuthenticationService authenticationService, IUserService userService, ILogger<UserController> logger)
        {

            _activationService = activationService;
            _authenticationService = authenticationService;
            _userService = userService;
            _logger = logger;
        }


        [HttpGet("Activation/{activationCode}")]
        public async Task<IActionResult> ActivateUser(string activationCode)
        {
            try
            {
                var activationResult = await _activationService.ActivateUser(activationCode);
                return Ok(activationResult);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong, due to {ex.Message}, please try again");
            }

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> AuthenticateUserLogin([FromBody] LoginRequestDto loginDetails)
        {
            try
            {
                var loginResult = await _authenticationService.AuthenticateUserLogin(loginDetails.Email, loginDetails.Password);
                return Ok(loginResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            try
            {
                var createUserResult = await _userService.CreateUser(user);
                if (createUserResult)
                    return Ok(createUserResult);
                return BadRequest("Unable to create user. Please try again later");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create user");
                return BadRequest("Unable to create user. Please check data and try again");
            }
        }


        [HttpPost]
        [Route("/changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel passwordDetails)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(passwordDetails.CurrentPassword, passwordDetails.NewPassword);
                if (result)
                    return Ok("Password has been changed successfully.");
                return BadRequest("Unable to Change password. Please try again later");                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }

}
   

