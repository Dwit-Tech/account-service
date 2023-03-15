using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService()
        {
        }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public async Task<bool> SendMailAsync(string userId, string fromEmail, string toEmail, string subject, string body, string cc, string bcc)
        {
            var emailObject = new Email { FromEmail = fromEmail, ToEmail = toEmail, Subject = subject, Body = body };

           

            var client = new RestClient(_configuration["notificationService:url"]);
            var request = new RestRequest(_configuration["notificationService:url"], Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var message = JsonConvert.SerializeObject(emailObject);
            request.AddBody(message, "application/json");
            RestResponse response = await client.ExecuteAsync(request);

            if (response is null)
                return false;

            return true;

        }
    }

}
