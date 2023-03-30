using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendMailAsync(Email email) //TODO
        {
            return true;
        }

        public Email GenerateEmail(UserDto user)
        {
            return new Email
            {
                FromEmail = _configuration["GMAIL_INFO:EMAIL"],
                ToEmail = user.Email,
                Body = "",
                Subject = "User Activation Email"
            };
        }
    }
}
