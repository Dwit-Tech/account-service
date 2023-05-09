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
        [AllowAnonymous]
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
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateUserLogin([FromBody] LoginRequestDto loginDetails)
        {
            try
            {
                var loginResult = await _authenticationService.AuthenticateUserLogin(loginDetails.Email, loginDetails.Password);
                if (loginResult == null)
                {
                    return BadRequest("Invalid email or password.");
                }
                return Ok(new { message = "Login successful." });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            try
            {
                var createUserResult = await _userService.CreateUser(user);
                if (createUserResult)
                    return new CreatedResult("User", null);
                return BadRequest("Unable to create user. Please try again later");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create user");
                return BadRequest("Unable to create user. Please check data and try again");
            }
        }


        [HttpPost("changepassword")]
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
                _logger.LogError(ex, "Unable to change password");
                return BadRequest("Unable to Change password. Please try again later");
            }
        }


        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                var logoutResult = await _userService.LogoutUser(authHeader);
                if (logoutResult)
                    return Ok(logoutResult);

                return BadRequest("No authorization header present");
            }
            catch (Exception e)
            {
                throw new Exception($"{e}");
            }
        }


        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            try
            {
                var result = await _userService.ResetPassword(email);
                if (result)
                    return Ok("An e-mail has been sent to your email address. Please follow the instructions in the email to reset your password.");
                return BadRequest("Unable to Reset password. Please try again later");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Send password reset email");
                return BadRequest("Unable to Reset password. Please try again later");
            }            
        }
    }

}
   

