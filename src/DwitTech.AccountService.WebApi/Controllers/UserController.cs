using AutoMapper;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
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
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IActivationService activationService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _activationService = activationService;
            _configuration = configuration;
            _mapper = mapper;
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
                throw new Exception(ex.Message);
            }

        }
    }
}
