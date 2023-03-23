using System.Text;
using System.Text.Json;
using DwitTech.AccountService.Core.Enums;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpointUrl;

        public ActivationService(IConfiguration configuration, IUserRepository userRepository, IHttpClientFactory httpClientFactory, string endpointUrl)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _endpointUrl = _configuration["NOTIFICATIONSERVICE_SENDMAILENDPOINT"];
        }

        private static string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration["BASE_URL"];
        }

        private async Task<string> GetActivationUrl(int userId)
        {
            string baseUrl = GetBaseUrl();
            string activationCode = GetActivationCode();
            try
            {
                var validationCode = new ValidationCode
                {
                    Code = activationCode,
                    CodeType = CodeType.Activation,
                    UserId = userId,
                    NotificationChannel = NotificationChannel.Email,
                    ModifiedOnUtc = DateTime.UtcNow
                };
                var saveResponse = await _userRepository.SaveUserValidationCode(validationCode);
                string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
                return activationUrl;
            }
            catch (Exception ex)
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

        public async Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email)
        {
            const string subject = "Account Activation";
            email.Subject = subject;
            var activationUrl = await GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            email.Body = templateText;
            var response = await SendMailAsync(email);
            return response;
        }

        public async Task<bool> SendMailAsync(Email email)
        {
            var serializedEmail = JsonSerializer.Serialize(email);
            var content = new StringContent(serializedEmail, Encoding.UTF8, "application/json");

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.BaseAddress = new Uri(_configuration["NOTIFICATIONSERVICE_BASEURL"]);
                var response = await httpClient.PostAsync(_endpointUrl, content);
                return response.IsSuccessStatusCode;
            }
        }
    }
}