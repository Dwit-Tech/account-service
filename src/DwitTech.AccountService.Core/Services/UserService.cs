using DwitTech.AccountService.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace DwitTech.AccountService.Core.Services
{
  
    public class UserService : IUserService
    {
        private readonly ActivationService _activationService;
        public UserService(ActivationService activationService)
        {
            _activationService = activationService;
        }
        public bool SendActivationEmail(string email)
        {
            email = "rumiklint529@gmail.com";
            var actvationEmail = _activationService.SendActivationEmail
                ("maryann.pclng@gmail.com", email, "SampleTemplateHTML", "Joe Doe", "", "", "");
            if (actvationEmail)
            {
                return true;
            }
            return false;
        }
    }


} 
