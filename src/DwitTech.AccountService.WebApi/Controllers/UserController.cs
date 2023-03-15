using DwitTech.AccountService.Data.Entities;
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
                string fromEmail = "support@gmail";
                string toEmail = "info@gmail.com";
                string templateName = "WelcomeEmail";
                string subject = "Account Details";
                string cc = "";
                string bcc = "";

                var activationResult = await _activationService.ActivateUser(activationCode, fromEmail , toEmail, templateName, subject, cc, bcc);
                return Ok(activationResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error. Something went Wrong!");
            }

        } 
        
    }

}
