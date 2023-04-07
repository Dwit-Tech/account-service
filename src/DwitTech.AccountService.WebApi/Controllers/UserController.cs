using Microsoft.AspNetCore.Mvc;
using DwitTech.AccountService.Core.Interfaces;

namespace DwitTech.AccountService.WebApi.Controllers
{ 
    public class UserController : BaseController
    {
        private readonly IActivationService _activationService;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IActivationService activationService, IAuthenticationService authenticationService)
        {

            _activationService = activationService;
            _authenticationService = authenticationService;
        }


        [HttpGet("/Activation/{activationCode}")]
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> AuthenticateUserLogin([FromBody] string email, string hashedPassword)
        {
            try
            {
                var loginResult = await _authenticationService.AuthenticateUserLogin(email, hashedPassword);
                return Ok(loginResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}
