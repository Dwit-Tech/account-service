using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration; //Config instance for GetBaseUrl method
        private readonly IEmailService _emailService;
        public ActivationService(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        private static string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration.GetSection("BaseUrl").Value;
        }

        private string GetActivationUrl(int userId)
        {
            string baseUrl = GetBaseUrl();
            string activationCode = GetActivationCode();
            string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
            return activationUrl;
        }

        private string GetTemplate(string templateName)
        {
            string trimmedTemplateName = templateName.Trim();
            string filePath = "Templates/" + trimmedTemplateName;
            StreamReader str = new StreamReader(filePath);
            var templateText = str.ReadToEnd();
            str.Close();
            return templateText.ToString();
        }

        public async Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email )
        {
            string subject = "Account Activation";
            email.Subject = subject;
            var activationUrl = GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName) ;
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            email.Body = templateText;
            var response = await _emailService.SendMailAsync(email);
            return response;
        }
    }
}