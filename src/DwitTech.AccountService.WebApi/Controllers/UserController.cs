﻿using Microsoft.AspNetCore.Mvc;
using DwitTech.AccountService.Core.Interfaces;

namespace DwitTech.AccountService.WebApi.Controllers
{ 
    public class UserController : BaseController
    {
        private readonly IActivationService _activationService;
        private readonly IUserService _userService;

        public UserController(IActivationService activationService, IUserService userService)
        {

            _activationService = activationService;
            _userService = userService;
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
        public async Task<IActionResult> UserLogin([FromBody] string email, string hashedPassword)
        {
            try
            {
                var loginResult = await _userService.UserLogin(email, hashedPassword);
                return Ok(new { message = "Login successful." });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}
