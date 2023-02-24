using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Security.Cryptography.X509Certificates;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;


        public ActivationService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;

        }

        private static string GetActivationCode()
        {
            var activationCode = RandomUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration.GetSection("BaseUrl").Value;
        }

        private async Task<string> GetActivationUrl(string userId)
        {
            string baseUrl = GetBaseUrl();
            string activationCode = GetActivationCode();
            var SaveResponse = await _userRepository.SaveUserValidationCode(userId, activationCode);
            if (SaveResponse is null)
                throw new Exception("Failed to save validation code");
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

        private async Task<bool> SendMailAsync(string fromEmail, string toEmail, string subject, string body, string cc = "", string bcc = "")
        {
            var emailObject = new Email { FromEmail = fromEmail, ToEmail = toEmail, Subject = subject, Body = body };

            var client = new RestClient(_configuration["NotificationService:BaseUrl"]);
            var request = new RestRequest(_configuration["NotificationService:SendEmail"]).AddJsonBody(emailObject);
            var response = await client.PostAsync<string>(request);
            if (request is null);

            return false;

        }

        public async Task<bool> SendActivationEmail(string userId, string fromEmail, string toEmail, string templateName, string RecipientName,
            string subject = "Account Activation", string cc = "", string bcc = "")
        {

            var baseUrl = GetBaseUrl();
            var activationUrl = await GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            string body = templateText;
            var response = await SendMailAsync(fromEmail, toEmail, subject, body, cc, bcc);

            return response;
        }
    }
}