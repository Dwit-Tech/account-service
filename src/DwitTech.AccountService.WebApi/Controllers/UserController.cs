﻿using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DwitTech.AccountService.Core.Interfaces;


namespace DwitTech.AccountService.WebApi.Controllers
{ 
    public class UserController : BaseController
    {
        private readonly IActivationService _activationService;

        public UserController(IActivationService activationService)
        {
           
            _activationService = activationService;
        }


        [HttpGet("/Activation/{activationCode}")]
        public async Task<IActionResult> ActivateUser(string activationCode)
        {
            try
            {
                var activationResult = await _activationService.ActivateUser(activationCode);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error. Something went Wrong!");
            }

        }


  
        
        
    }

}
