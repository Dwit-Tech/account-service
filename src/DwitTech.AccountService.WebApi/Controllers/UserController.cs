using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        } 
        
        [AllowAnonymous]
        [HttpPost]
        [Route("/createuser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            try
            {
                await _userService.CreateUser(user);
                return Ok("User Created Successfully");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
