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
        public async Task<IActionResult> ActivateUser(string activationCode, User user, string fromEmail , string toEmail, string templateName, string subject, string cc, string bcc)
        {
            try
            {
                var activationResult = await _activationService.ActivateUser(activationCode, user, fromEmail , toEmail, templateName, subject, cc, bcc);
                return Ok(activationResult);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong, due to {ex.Message}, please try again");
            }

        } 
        
    }

}
