using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

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
            try {
                var saveResponse = await _userRepository.SaveUserValidationCode(userId, activationCode);
                string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
                return activationUrl;
            }catch(Exception ex)
            {
                throw new ArgumentException($"{ex.Message}");
            }
            
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

        private async Task<bool> SendMailAsync(string fromEmail, string toEmail, string subject, string body, string cc, string bcc)
        {
            var emailObject = new Email { FromEmail = fromEmail, ToEmail = toEmail, Subject = subject, Body = body };

            var url = "https://jsonplaceholder.typicode.com/posts";


            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post); 
            request.AddHeader("Content-Type", "application/json");
            var message = JsonConvert.SerializeObject(emailObject);
            request.AddBody(message, "application/json");
            RestResponse response = await client.ExecuteAsync(request);

            if (response is null)
                return false;
            
            return true;

        }

        public async Task<bool> SendActivationEmail(string userId, string fromEmail, string toEmail, string templateName, string RecipientName,
            string subject = "Account Activation", string cc = "", string bcc = "")
        {

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